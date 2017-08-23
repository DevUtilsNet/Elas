using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using DevUtils.Elas.Tasks.Core.Build.Framework.Extensions;
using DevUtils.Elas.Tasks.Core.Common;
using DevUtils.Elas.Tasks.Core.IO;
using DevUtils.Elas.Tasks.Core.Xliff;
using DevUtils.Elas.Tasks.Core.Xliff.Extensions;
using Microsoft.Build.Framework;

namespace DevUtils.Elas.Tasks.Core.EmbeddedResources
{
	/// <summary> The elas import exists embedded resource. This class cannot be inherited. </summary>
	public sealed class ElasImportExistsEmbeddedResource : TaskExtension
	{
		/// <summary> Gets or sets target cultures. </summary>
		///
		/// <value> The target cultures. </value>
		public ITaskItem[] TargetCultures { get; set; }

		/// <summary> Gets or sets the embedded resource. </summary>
		///
		/// <value> The embedded resource. </value>
		public ITaskItem[] EmbeddedResource { get; set; }

		/// <summary> Gets or sets the intermediate documents. </summary>
		///
		/// <value> The intermediate documents. </value>
		public ITaskItem[] IntermediateDocuments { get; set; }

		#region Overrides of TaskExtension

		/// <summary>
		/// Try execute.
		/// </summary>
		protected override void TryExecute()
		{
			if (TargetCultures == null || EmbeddedResource == null || IntermediateDocuments == null)
			{
				return;
			}

			foreach (var item in IntermediateDocuments)
			{
				var documentFileInfo = new FileInfo(item.ItemSpec);
				if (documentFileInfo.Exists)
				{
					continue;
				}

				var sourceCulture = new CultureInfo(item.RequestMetadata("ElasSourceLanguage"));

				var xliffDocument = new XliffDocument();
				var documentSourceItemSpec = item.RequestMetadata("ElasSourceItemSpec");

				var documentWithoutExtension = Path2.GetFileNameWithDirectoryWithoutExtension(documentFileInfo.FullName);

				foreach (var item2 in TargetCultures.Select(s => new CultureInfo(s.ItemSpec)))
				{
					var sourcePath = Path2.ChangeFileNameWithoutExtension(documentWithoutExtension, Path.GetFileNameWithoutExtension(documentWithoutExtension) + "." + item2.Name);

					var source = EmbeddedResource.FirstOrDefault(w =>
						string.Equals(w.GetMetadata("WithCulture"), "true", StringComparison.InvariantCultureIgnoreCase) &&
						string.Equals(w.GetMetadata(MSBuildWellKnownItemMetadates.FullPath), sourcePath, StringComparison.InvariantCultureIgnoreCase));

					if (source == null)
					{
						continue;
					}

					var xliffFile = xliffDocument.CreateFile(documentSourceItemSpec, sourceCulture, item2, XliffDataType.Resx);
					xliffDocument.Files.Add(xliffFile);
					ElasMSResxExportToIntermediateDocument.ExportResx(Enumerable.Repeat(xliffFile, 1));

					using (var reader = new ResXResourceReader(source.ItemSpec))
					{
						reader.UseResXDataNodes = true;

						foreach (DictionaryEntry item3 in reader)
						{
							var resxData = (ResXDataNode)item3.Value;
							var key = item3.Key.ToString();
							var tu = xliffFile.Units.GetTransUnitByCompositeId(key);
							if (tu == null || tu.Target == null)
							{
								continue;
							}
							var value = (string)resxData.GetValue((ITypeResolutionService)null);
							tu.Target.Content = value;
							tu.Target.State = XliffTargetState.Translated;
						}

						Log.LogWarning(Log.FormatString("The file \"{0}\" has been successfully migrated, you can now delete this file from the project.", source.ItemSpec));
					}
				}

				if (xliffDocument.IsDirty)
				{
					xliffDocument.Save(documentFileInfo.FullName);
				}
			}
		}
		#endregion
	}
}