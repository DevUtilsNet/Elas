using System.IO;

namespace DevUtils.Elas.Tasks.Core.IO
{
	static class Path2
	{
		public static string ChangeFileName(string path, string fileName)
		{
			var ret = Path.Combine(Path.GetDirectoryName(path), fileName);
			return ret;
		}

		public static string ChangeFileNameWithoutExtension(string path, string fileName)
		{
			var ret = Path.Combine(Path.GetDirectoryName(path), fileName + Path.GetExtension(path));
			return ret;
		}

		public static string GetFileNameWithDirectoryWithoutExtension(string path)
		{
			var ret = Path.ChangeExtension(path, string.Empty).TrimEnd('.');
			return ret;
		}
	}
}