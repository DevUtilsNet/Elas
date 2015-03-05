using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using DevUtils.Elas.Pretranslate.MicrosoftTranslation.DataMarket.V2;
using DevUtils.Elas.Pretranslate.MicrosoftTranslation.DataMarket.V2.Extensions;
using DevUtils.Elas.Pretranslate.MicrosoftTranslation.V2;
using DevUtils.Elas.Tasks.Core;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Pretranslate.MicrosoftTranslation
{
	/// <summary> The elas pretranslate. </summary>
	public class ElasPretranslate : TaskExtension
	{
		private static string _token;
		private static DateTime _expiretTime;
		private static readonly object SyncRoot = new object();

		private bool _dirty;
		private string _clientId;
		private string _clientSecret;
		

		/// <summary> Gets the files. </summary>
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Gets or sets the configuration file. </summary>
		///
		/// <value> The configuration file. </value>
		[Required]
		public string ConfigurationPath { get; set; }

		private string GetAppId()
		{
			if (string.IsNullOrEmpty(_token) || _expiretTime <= DateTime.Now)
			{
				lock (SyncRoot)
				{
					if (string.IsNullOrEmpty(_token) || _expiretTime <= DateTime.Now)
					{
						using (var factory = new WebChannelFactory<IDataMarket>(new Uri("https://datamarket.accesscontrol.windows.net")))
						{
							var channel = factory.CreateChannel();

							var result = channel.Auth(
								_clientId ?? 
#if DEBUG
								"MAT"
#else
								"ELAS"
#endif
								,
								_clientSecret ?? 
#if DEBUG
								"????"
#else
								"????"
#endif
								,
								"http://api.microsofttranslator.com",
								"client_credentials");

							_expiretTime = DateTime.Now + TimeSpan.FromSeconds(result.ExpiresIn - 10);
							_token = "Bearer " + result.AccessToken;
						}
					}
				}
			}

			return _token;
		}

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			var project = ProjectCollection.GlobalProjectCollection.LoadProject(ConfigurationPath);

			var propVal = project.GetPropertyValue("ElasMicrosoftTranslationClientId");

			if (!string.IsNullOrEmpty(propVal))
			{
				_clientId = propVal;
			}

			propVal = project.GetPropertyValue("ElasMicrosoftTranslationClientSecret");

			if (!string.IsNullOrEmpty(propVal))
			{
				_clientSecret = propVal;
			}

			foreach (var item in Files)
			{
				Pretranslate(item);
			}
		}

		private void Pretranslate(ITaskItem taskItem)
		{
			var file = taskItem.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath);
			var xliffDocument = new XliffDocument();
			xliffDocument.Load(file);

			using (var factory = new ChannelFactory<ILanguageService>(
				new BasicHttpBinding(BasicHttpSecurityMode.Transport), 
				"https://api.microsofttranslator.com/V2/soap.svc"))
			{
				foreach (var item in xliffDocument.Files)
				{
					Log.LogMessage(MessageImportance.High, "\"{0}\" translate {1} -> {2} ...", taskItem, item.SourceLanguage.Name, item.TargetLanguage.Name);
					PretranslateFile(factory, item);
				}
			}

			if (_dirty)
			{
				xliffDocument.Save(file);
				Log.LogMessage(MessageImportance.Low, Log.FormatString("Intermediate document \"{0}\" was updated.", taskItem));
			}
		}

		private void PretranslateFile(ChannelFactory<ILanguageService> factory, XliffFile xliffFile)
		{
			var options = new TranslateOptions
			{
				IncludeMultipleMTAlternatives = true
			};

			Parallel.ForEach(
				xliffFile.Units.GetAllUnits().OfType<XliffTransUnit>(), 
				() => Tuple.Create(xliffFile, factory.CreateChannel(), options), Translate,
				s => { });
		}

		private Tuple<XliffFile, ILanguageService, TranslateOptions> Translate(
			XliffTransUnit item, 
			ParallelLoopState state, 
			Tuple<XliffFile, ILanguageService, TranslateOptions> args)
		{
			if (state.ShouldExitCurrentIteration)
			{
				return args;
			}

			if (item.Target.IsShouldBeTranslated())
			{
				var response = args.Item2.GetTranslations(GetAppId(),
					item.Source.Content,
					args.Item1.SourceLanguage.Name,
					args.Item1.TargetLanguage.Name, 1, args.Item3);
				var trans = response.Translations.FirstOrDefault();
				if (trans != null && item.Target.Content != trans.TranslatedText)
				{
					item.Target.Content = trans.TranslatedText;
					item.Target.State = XliffTargetState.NeedsReviewTranslation;
					_dirty = true;
				}
			}
			return args;
		}
	}
}