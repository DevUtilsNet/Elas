using System;
using System.Globalization;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.IO.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	/// <summary> A class. This class cannot be inherited. </summary>
	public sealed class ElasExportToIntermediateDocumentResourceCompile : TaskExtension
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

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null || TargetCultures == null)
			{
				return;
			}

			foreach (var item in Files)
			{
				Export(item);
			}
		}

		private void Export(ITaskItem taskItem)
		{
			var extension = taskItem.GetMetadata(MSBuildWellKnownItemMetadates.Extension);
			if (extension.Equals(".rc", StringComparison.InvariantCultureIgnoreCase))
			{
				var document = taskItem.RequestMetadata("ElasIntermediateDocumentPath");
				var documentFileInfo = new FileInfo(document);
				if (!Log.ShouldBuild(new[] { new FileInfo(ConfigurationPath), new FileInfo(taskItem.ItemSpec) }, Enumerable.Repeat(documentFileInfo, 1)))
				{
					return;
				}

				var xliffDocument = new XliffDocument();
				if (documentFileInfo.Exists)
				{
					xliffDocument.Load(documentFileInfo.FullName);
				}

				var exporter = new RCExporterToIntermediateDocument();
				var sourceLanguage = new CultureInfo(taskItem.RequestMetadata("ElasSourceLanguage"));

				var files = TargetCultures.Select(s => new CultureInfo(s.ToString()))
				                          .Select(s => xliffDocument.Files.GetOrCreateFile(taskItem.ItemSpec, sourceLanguage, s, XliffDataType.Winres))
				                          .ToArray();

				exporter.Export(files);

				foreach (var item2 in files)
				{
					item2.UpdateDate();
					item2.UpdateAbsentFlag();
				}

				if (xliffDocument.Export(documentFileInfo))
				{
					Log.LogMessage(MessageImportance.Low, Log.FormatString("Intermediate document \"{0}\" was rewritten.", documentFileInfo.GetDisplayPath()));
					AddFilesToProject(taskItem);
				}
			}
		}

		private void AddFilesToProject(ITaskItem file)
		{
			var ti = new TaskItem(file.RequestMetadata("ElasIntermediateDocumentPath"));
			ti.SetMetadata("DependentUpon", file.ItemSpec);

			var task = CreateTask<ElasAddFilesToProject>();

			task.Files = new ITaskItem[] { ti };

			task.Execute();
		}
	}
}
