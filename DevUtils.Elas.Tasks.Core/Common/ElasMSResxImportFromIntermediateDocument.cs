using System;
using System.Collections;
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

namespace DevUtils.Elas.Tasks.Core.Common
{
	/// <summary> The elas milliseconds resx import from intermediate document. </summary>
	public class ElasMSResxImportFromIntermediateDocument : TaskExtension
	{
		/// <summary> Gets or sets the resources. </summary>
		/// <value> The resources. </value>
		public ITaskItem[] Files { get; set; }

		/// <summary> Try execute. </summary>
		protected override void TryExecute()
		{
			if (Files == null)
			{
				return;
			}

			foreach (var item in Files.GroupBy(g => g.GetMetadata("ElasIntermediateDocumentPath").ToLower()))
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					var documentFileInfo = new FileInfo(item.Key);

					XliffDocument xliffDocument = null;

					foreach (var item2 in item)
					{
						var sourceFileInfo = new FileInfo(item2.ItemSpec);

						if (!Log.ShouldBuild(documentFileInfo, sourceFileInfo))
						{
							continue;
						}

						if (xliffDocument == null)
						{
							xliffDocument = new XliffDocument();
							xliffDocument.Load(documentFileInfo.FullName);
						}

						var sourceCulture = new CultureInfo(item2.RequestMetadata("ElasSourceLanguage"));
						var targetCulture = new CultureInfo(item2.RequestMetadata("ElasTargetLanguage"));

						var xliffFile = xliffDocument.Files.FirstOrDefault(f => Equals(f.SourceLanguage, sourceCulture) && Equals(f.TargetLanguage, targetCulture));

						var sourceFile = item2.RequestMetadata("ElasSourceItemSpec");

						var directory = Path.GetDirectoryName(item2.ItemSpec);
						Directory.CreateDirectory(directory);

						try
						{
							using (var writer = new ResXResourceWriter(item2.ItemSpec))
							{
								if (xliffFile != null)
								{
									ImportResx(sourceFile, xliffFile, writer);
								}
							}
						}
						catch (Exception)
						{
							new FileInfo(item2.ItemSpec).QuietDelete();
							throw;
						}
						
					}
				}
			}
		}

		private void ImportResx(string sourceFile, XliffFile xliffFile, ResXResourceWriter targetResX)
		{
			using (var reader = new ResXResourceReader(sourceFile))
			{
				reader.UseResXDataNodes = true;
				foreach (DictionaryEntry item in reader)
				{
					var resxData = (ResXDataNode) item.Value;

					var typeName = resxData.GetValueTypeName((ITypeResolutionService) null);
					if (typeName != null && typeName.StartsWith("System.String"))
					{
						var unit = xliffFile.Units.GetTransUnitByCompositeId(item.Key.ToString());
						if (unit != null)
						{
							if (IsTranslated(unit))
							{
								var newResxData = new ResXDataNode(unit.Id, unit.Target.Content);
								targetResX.AddResource(newResxData);
							}
						}
					}
				}
			}
		}

		private static bool IsTranslated(XliffTransUnit unit)
		{
			var ret = unit.IsTranslated();
			return ret;
		}
	}
}
