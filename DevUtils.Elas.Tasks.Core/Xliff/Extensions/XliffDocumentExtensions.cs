using System.IO;
using DevUtils.Elas.Tasks.Core.IO.Extensions;

namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	static class XliffDocumentExtensions
	{
		public static void SaveNotChangeFileTime(this XliffDocument xliffDocument, string filename)
		{
			var documentFileInfo = new FileInfo(filename);

			var lastWriteTimeUtc = documentFileInfo.LastWriteTimeUtc;

			xliffDocument.Save(documentFileInfo.FullName);

			documentFileInfo.LastWriteTimeUtc = lastWriteTimeUtc;
		}

		public static bool Export(this XliffDocument xliffDocument, FileInfo file)
		{
			if (file.Exists && !xliffDocument.IsDirty)
			{
				if (file.IsReadOnly)
				{
					var oldAttr = file.Attributes;
					file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;
					try
					{
						file.Touch();
					}
					finally
					{
						file.Attributes = oldAttr;
					}
				}
				else
				{
					file.Touch();
				}

				return false;
			}

			file.DTEClearReadOnly();

			file.Refresh();

			xliffDocument.Save(file.FullName);

			return true;
		}
	}
}
