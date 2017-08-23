using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup.Localizer;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.IO.Extensions;
using DevUtils.Elas.Tasks.Core.Windows.Markup.Localizer;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.PageMarkup
{
	/// <summary> The elas export to intermediate document page markup. This class cannot be
	/// inherited. </summary>
	public sealed class ElasExportToIntermediateDocumentPageMarkup : ElasImportExportIntermediateDocumentPageMarkup
	{
		private string _currentXamlFile;

		/// <summary> Gets or sets the page markup. </summary>
		///
		/// <value> The page markup. </value>
		public ITaskItem[] PageMarkup { get; set; }

		/// <summary> Gets or sets the configuration file. </summary>
		///
		/// <value> The configuration file. </value>
		[Required]
		public string ConfigurationPath { get; set; }

		/// <summary> Gets or sets target cultures. </summary>
		///
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }

		/// <summary> Process this object. </summary>
		protected override void Process()
		{
			if (PageMarkup == null || TargetCultures == null)
			{
				return;
			}

			var processedFiles = new List<ITaskItem>(PageMarkup.Length);

			foreach (var item in PageMarkup)
			{
				var intermediateDocumentPath = item.RequestMetadata("ElasIntermediateDocumentPath");

				var pmFile = new FileInfo(item.ItemSpec);
				var sourceFile = new FileInfo(item.RequestMetadata("ElasGeneratedBaml"));
				var targetFile = new FileInfo(intermediateDocumentPath);
				if (!Log.ShouldBuild(new[] { new FileInfo(ConfigurationPath), pmFile }, Enumerable.Repeat(targetFile, 1)))
				{
					continue;
				}

				var sourceCulture = new CultureInfo(item.RequestMetadata("ElasSourceLanguage"));

				Log.LogMessage(MessageImportance.Low, Log.FormatString("Processing file \"{0}\"", sourceFile.GetDisplayPath()));

				_currentXamlFile = item.ItemSpec;

				using (var ss = sourceFile.OpenRead())
				{
					var localizer = new BamlLocalizer(ss, new BamlLocalizabilityResolverByReflection(Localizability));
					localizer.ErrorNotify += OnLocalizerErrorNotify;
					var resources = localizer.ExtractResources();

					var processedFile = new TaskItem(item);
					processedFiles.Add(processedFile);

					var documentFileInfo = targetFile;
					var xliffDocument = new XliffDocument();
					if (documentFileInfo.Exists)
					{
						xliffDocument.Load(documentFileInfo.FullName);
					}
					else
					{
						processedFile.SetMetadata("ElasWithNewIntermediateDocument", "True");
					}

					var xamlFile = item.ItemSpec;
					var files = TargetCultures.Select(s => new CultureInfo(s.ToString()))
					                          .Select(s => xliffDocument.Files.GetOrCreateFile(xamlFile, sourceCulture, s, XliffDataType.Xml))
					                          .ToArray();

					foreach (var item2 in resources)
					{
						var key = (BamlLocalizableResourceKey)item2.Key;
						var value = (BamlLocalizableResource)item2.Value;

						if (!IsLocalizable(value))
						{
							continue;
						}

						var id = GetId(key);

						foreach (var item3 in files)
						{
							bool updatedOrCreated;
							var tu = item3.Units.UpdateOrCreateTransUnitByCompositeId(id, value.Content, out updatedOrCreated);
							tu.Note = value.Comments;

							if (updatedOrCreated && !value.Modifiable)
							{
								tu.NotTranslate = true;
								tu.Target.Content = tu.Source.Content;
								tu.Target.State = XliffTargetState.Final;
							}
						}
					}

					foreach (var item2 in files)
					{
						item2.UpdateDate();
						item2.UpdateAbsentFlag();
					}

					if (xliffDocument.Export(documentFileInfo))
					{
						Log.LogMessage(MessageImportance.Low, Log.FormatString("Intermediate document \"{0}\" was rewritten.", documentFileInfo.GetDisplayPath()));

						AddFilesToProject(item);
					}
				}

				_currentXamlFile = null;
			}

			ProcessedFiles = processedFiles.ToArray();
		}

		private void AddFilesToProject(ITaskItem pageMarkup)
		{
			var ti = new TaskItem(pageMarkup.RequestMetadata("ElasIntermediateDocumentPath"));
			ti.SetMetadata("DependentUpon", pageMarkup.ItemSpec);

			var task = CreateTask<ElasAddFilesToProject>();

			task.Files = new ITaskItem[] { ti };

			task.Execute();
		}

		private static bool IsLocalizable(BamlLocalizableResource resource)
		{
			if (!resource.Readable)
			{
				return false;
			}

			//if (!resource.Modifiable)
			//{
			//	return false;
			//}

			var ret = !String.IsNullOrEmpty(resource.Content) &&
			          //resource.Category != LocalizationCategory.None &&
			          resource.Category != LocalizationCategory.NeverLocalize &&
			          resource.Category != LocalizationCategory.Ignore &&
			          resource.Category != LocalizationCategory.Font &&
			          resource.Category != LocalizationCategory.XmlData &&
			          resource.Category != LocalizationCategory.Hyperlink;
			return ret;
		}

		private void OnLocalizerErrorNotify(object sender, BamlLocalizerErrorNotifyEventArgs args)
		{
			var error = true;

			string message;

			switch (args.Error)
			{
				case BamlLocalizerError.DuplicateUid:
				{
					message = "More than one element has the same Uid value";
					break;
				}
				case BamlLocalizerError.DuplicateElement:
				{
					message = "The localized BAML contains more than one reference to the same element";
					break;
				}
				case BamlLocalizerError.IncompleteElementPlaceholder:
				{
					message = "The element's substitution contains incomplete child placeholders";
					break;
				}
				case BamlLocalizerError.InvalidCommentingXml:
				{
					message = "XML comments do not have the correct format";
					break;
				}
				case BamlLocalizerError.InvalidLocalizationAttributes:
				{
					message = "The localization commenting text contains invalid attributes";
					break;
				}
				case BamlLocalizerError.InvalidLocalizationComments:
				{
					message = "The localization commenting text contains invalid comments";
					break;
				}
				case BamlLocalizerError.InvalidUid:
				{
					message = "The Uid does not correspond to any element in the BAML source";
					break;
				}
				case BamlLocalizerError.MismatchedElements:
				{
					message = "Indicates a mismatch between substitution and source. The substitution must contain all the element placeholders in the source";
					break;
				}
				case BamlLocalizerError.SubstitutionAsPlaintext:
				{
					message = "The substitution of an element's content cannot be parsed as XML, therefore any formatting tags in the substitution are not recognized. The substitution is instead applied as plain text";
					break;
				}
				case BamlLocalizerError.UidMissingOnChildElement:
				{
					error = false;
					message = "A child element does not have a Uid. As a result, it cannot be represented as a placeholder in the parent's content string";
					break;
				}
				case BamlLocalizerError.UnknownFormattingTag:
				{
					message = "A formatting tag in the substitution is not recognized";
					break;
				}
				default:
				throw new ArgumentOutOfRangeException();
			}

			if (args.Key != null && !string.IsNullOrEmpty(args.Key.Uid))
			{
				message = string.Format("{0}. Uid = \"{1}\".", message, args.Key.Uid);
			}

			if (error)
			{
				throw new TaskException(_currentXamlFile, message);
			}

			Log.LogWarning("BamlLocalizer", args.Error.ToString(), null, _currentXamlFile, 0, 0, 0, 0, message);
		}
	}
}
