using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Build.Utilities.Extensions;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	/// <summary> The elas import from intermediate document resource compile. This class cannot be
	/// inherited. </summary>
	public sealed class ElasImportFromIntermediateDocumentResourceCompile : TaskExtension
	{
		/// <summary> Gets or sets the files. </summary>
		///
		/// <value> The files. </value>
		public ITaskItem[] Files { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			foreach (var item in Files
				.Where(w => w.GetMetadata(MSBuildWellKnownItemMetadates.Extension).Equals(".rc", StringComparison.InvariantCultureIgnoreCase))
				.GroupBy(g => g.RequestMetadata("ElasIntermediateDocumentPath").ToLower()))
			{
				Import(item.Key, item);
			}
		}

		#endregion

		private void Import(string intermediateDocumentPath, IEnumerable<ITaskItem> taskItems)
		{
			var taskItems2 = taskItems.ToArray();

			var documentFileInfo = new FileInfo(intermediateDocumentPath);
			if (!Log.ShouldBuild(Enumerable.Repeat(documentFileInfo, 1), taskItems2.Select(s => new FileInfo(s.ItemSpec))))
			{
				return;
			}

			var xliffDocument = new XliffDocument();
			xliffDocument.Load(documentFileInfo.FullName);

			var withCultures = taskItems2.Select(
				s =>
					Tuple.Create(s,
						new CultureInfo(s.RequestMetadata("ElasSourceLanguage")),
						new CultureInfo(s.RequestMetadata("ElasTargetLanguage")))).ToArray();

			var list = new List<RCImportItemInfo>(withCultures.Length);

			foreach (var item in xliffDocument.Files)
			{
				list.AddRange(withCultures.Where(w =>
					Equals(w.Item2, item.SourceLanguage) &&
					Equals(w.Item3, item.TargetLanguage)).Select(s =>
						new RCImportItemInfo
						{
							OutFile = s.Item1.ToString(),
							XliffFile = item
						}));
			}

			var importer = new RCImporterFromIntermediateDocument();
			importer.Import(list);
		}
	}
}