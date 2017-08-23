using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Markup;
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
	/// <summary> The elas import from intermediate document page markup. This class cannot be
	/// inherited. </summary>
	public sealed class ElasImportFromIntermediateDocumentPageMarkup : ElasImportExportIntermediateDocumentPageMarkup
	{
		/// <summary> Gets or sets the page markup. </summary>
		///
		/// <value> The page markup. </value>
		public ITaskItem[] SatelliteBaml { get; set; }

		#region Overrides of TaskExtension

		/// <summary> Process this object. </summary>
		protected override void Process()
		{
			var processedFiles = new List<ITaskItem>(SatelliteBaml.Length);

			foreach (var item in SatelliteBaml.GroupBy(g => g.RequestMetadata("ElasIntermediateDocumentPath").ToLower()))
			{
				var intermediateDocumentPath = item.Key;
				var intermediateDocument = new FileInfo(intermediateDocumentPath);

				XliffDocument xliffDocument = null;

				foreach (var item2 in item)
				{
					var targetFile = new FileInfo(item2.ItemSpec);
					if (!Log.ShouldBuild(intermediateDocument, targetFile))
					{
						continue;
					}

					processedFiles.Add(new TaskItem(item2));

					if (xliffDocument == null)
					{
						xliffDocument = new XliffDocument();
						xliffDocument.Load(intermediateDocument.FullName);
					}

					var sourceCulture = new CultureInfo(item2.RequestMetadata("ElasSourceLanguage"));
					var targetCulture = new CultureInfo(item2.RequestMetadata("ElasTargetLanguage"));

					var sourceFile = new FileInfo(item2.RequestMetadata("ElasGeneratedBaml"));
					var xliffFile = xliffDocument.Files.FirstOrDefault(f => f.Units.Any() && Equals(f.SourceLanguage, sourceCulture) && Equals(f.TargetLanguage, targetCulture));

					if (xliffFile != null)
					{
						BamlLocalizer localizer;
						BamlLocalizationDictionary resources;

						using (var fs = sourceFile.OpenRead())
						{
							localizer = new BamlLocalizer(fs, new BamlLocalizabilityResolverByReflection(Localizability));
							resources = localizer.ExtractResources();
						}

						var updateBaml = false;

						foreach (var item3 in resources)
						{
							var key = (BamlLocalizableResourceKey)item3.Key;
							var value = (BamlLocalizableResource)item3.Value;

							var id = GetId(key);

							var tu = xliffFile.Units.GetTransUnitByCompositeId(id);
							if (tu != null)
							{
								if (tu.IsTranslated())
								{
									updateBaml = true;
									value.Content = tu.Target.Content;
								}
							}
						}

						var xamlFile = item2.RequestMetadata("ElasSourceItemSpec");

						try
						{
							try
							{
								if (updateBaml)
								{
									using (var fs = targetFile.Create())
									{
										localizer.UpdateBaml(fs, resources);
									}
									continue;
								}
							}
							catch (XamlParseException e)
							{
								throw new TaskException(xamlFile, string.Format("\"{0}\" Note: Try to replace xml namespace definition to clr namespace.", e.Message), e);
							}
						}
						catch
						{
							targetFile.QuietDelete();
							throw;
						}
					}

					File.Copy(sourceFile.FullName, targetFile.FullName, true);
					targetFile.Touch();
				}

			}
			ProcessedFiles = processedFiles.ToArray();
		}

		#endregion
	}
}