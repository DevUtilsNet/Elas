using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.IO.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.Common
{
	/// <summary> The elas milliseconds resx export to intermediate document. </summary>
	public class ElasMSResxExportToIntermediateDocument : TaskExtension
	{
		/// <summary> Gets or sets the resources. </summary>
		/// <value> The resources. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Gets or sets the configuration file. </summary>
		///
		/// <value> The configuration file. </value>
		[Required]
		public string ConfigurationPath { get; set; }

		/// <summary> Gets or sets target cultures. </summary>
		///
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }

		/// <summary> Gets or sets the processed files. </summary>
		///
		/// <value> The processed files. </value>
		[Output]
		public ITaskItem[] ProcessedFiles { get; set; }

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null || TargetCultures == null)
			{
				return;
			}

			var processedFiles = new List<ITaskItem>(Files.Length);

			foreach (var item in Files)
			{
				var sourceFileInfo = new FileInfo(item.ItemSpec);
				var document = item.RequestMetadata("ElasIntermediateDocumentPath");
				var documentFileInfo = new FileInfo(document);

				if (!Log.ShouldBuild(new[] { new FileInfo(ConfigurationPath), sourceFileInfo }, Enumerable.Repeat(documentFileInfo, 1)))
				{
					continue;
				}

				var processedFile = new TaskItem(item);
				processedFiles.Add(processedFile);

				var xliffDocument = new XliffDocument();
				if (documentFileInfo.Exists)
				{
					xliffDocument.Load(documentFileInfo.FullName);
				}
				else
				{
					processedFile.SetMetadata("ElasWithNewIntermediateDocument", "True");
				}

				var sourceCulture = new CultureInfo(item.RequestMetadata("ElasSourceLanguage"));

				var original = item.ItemSpec;
				var files = TargetCultures.Select(s => new CultureInfo(s.ToString()))
				                          .Select(s => xliffDocument.Files.GetOrCreateFile(original, sourceCulture, s, XliffDataType.Resx))
				                          .ToArray();

				ExportResx(files);

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

			ProcessedFiles = processedFiles.ToArray();
		}

		private void AddFilesToProject(ITaskItem file)
		{
			var ti = new TaskItem(file.RequestMetadata("ElasIntermediateDocumentPath"));
			ti.SetMetadata("DependentUpon", file.ItemSpec);

			var task = CreateTask<ElasAddFilesToProject>();

			task.Files = new ITaskItem[] { ti };

			task.Execute();
		}

		internal static void ExportResx(IEnumerable<XliffFile> files)
		{
			foreach (var item in files.GroupBy(g => g.Original.ToLower()))
			{
				using (var reader = new ResXResourceReader(item.Key))
				{
					reader.UseResXDataNodes = true;

					foreach (DictionaryEntry item2 in reader)
					{
						var resxData = (ResXDataNode)item2.Value;

						if (resxData.FileRef != null)
						{
							continue;
						}

						var typeName = resxData.GetValueTypeName((ITypeResolutionService)null);
						if (typeName != null && typeName.StartsWith("System.String"))
						{
							foreach (var item3 in item)
							{
								var file = item3;

								var key = item2.Key.ToString();

								var value = (string)resxData.GetValue((ITypeResolutionService)null);
								bool updatedOrCreated;
								var tu = file.Units.UpdateOrCreateTransUnitByCompositeId(key, value, out updatedOrCreated);
								tu.Note = resxData.Comment;

								if (updatedOrCreated && key.StartsWith(">>"))
								{
									tu.NotTranslate = true;
									tu.Target.Content = tu.Source.Content;
									tu.Target.State = XliffTargetState.Translated;
								}
							}
						}
					}
				}
			}
		}
	}
}
