using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core
{
	/// <summary> The elas pretranslate. This class cannot be inherited. </summary>
	public sealed class ElasPretranslate : TaskExtension
	{
		private static readonly Dictionary<Tuple<CultureInfo, CultureInfo>, Dictionary<string, string>> TranslationsStorage = new Dictionary<Tuple<CultureInfo, CultureInfo>, Dictionary<string, string>>();

		/// <summary> Gets the files. </summary>
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Gets or sets the configuration file. </summary>
		///
		/// <value> The configuration file. </value>
		[Required]
		public string ConfigurationPath { get; set; }

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			foreach (var item in Files)
			{
				var file = item.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath);
				var xliffDocument = new XliffDocument();
				xliffDocument.Load(file);

				foreach (var item2 in xliffDocument.Files)
				{
					var key = Tuple.Create(item2.SourceLanguage, item2.TargetLanguage);
					Dictionary<string, string> translations;
					if (!TranslationsStorage.TryGetValue(key, out translations))
					{
						translations = new Dictionary<string, string>();
						TranslationsStorage[key] = translations;
					}

					foreach (var item3 in item2.Units.GetAllUnits().OfType<XliffTransUnit>().Where(TranslatedTransUnit))
					{
						translations[item3.Source.Content] = item3.Target.Content;
					}

					foreach (var item3 in item2.Units.GetAllUnits().OfType<XliffTransUnit>().Where(IsShouldBeTranslated))
					{
						string translate;
						if (translations.TryGetValue(item3.Source.Content, out translate))
						{
							item3.Target.Content = translate;
							item3.Target.State = XliffTargetState.NeedsReviewTranslation;
						}
					}
				}

				if (xliffDocument.IsDirty)
				{
					xliffDocument.Save(file);
					Log.LogMessage(MessageImportance.Low, Log.FormatString("Intermediate document \"{0}\" was updated.", item));
				}
			}
		}

		private static bool TranslatedTransUnit(XliffTransUnit unit)
		{
			var ret = unit.Target != null 
				&& unit.Target.State.HasValue 
				&& unit.Target.State.Value == XliffTargetState.Translated;
			return ret;
		}

		private static bool IsShouldBeTranslated(XliffTransUnit unit)
		{
			var ret = unit.Target.IsShouldBeTranslated();
			return ret;
		}
	}
}