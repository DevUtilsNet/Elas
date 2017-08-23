using System.IO;

namespace DevUtils.Elas.Tasks.Core.IO.Extensions
{
	/// <summary> A file system information extensions. This class cannot be inherited. </summary>
	public static class FileSystemInfoExtensions
	{
		/// <summary> A FileSystemInfo extension method that gets relative path. </summary>
		///
		/// <param name="systemInfo"> The systemInfo to act on. </param>
		/// <param name="to">				  to. </param>
		///
		/// <returns> The relative path. </returns>
		public static string GetRelativePath(this FileSystemInfo systemInfo, string to)
		{
			var ret = NativeMethods.GetRelativePath(systemInfo.FullName, systemInfo.Attributes, to, File.GetAttributes(to));

			return ret;
		}

		/// <summary> A FileSystemInfo extension method that gets display path. </summary>
		///
		/// <param name="systemInfo"> The systemInfo to act on. </param>
		///
		/// <returns> The display path. </returns>
		public static string GetDisplayPath(this FileSystemInfo systemInfo)
		{
			var ret = NativeMethods.GetRelativePath(Directory.GetCurrentDirectory(), FileAttributes.Directory, systemInfo.FullName, systemInfo.Attributes);
			return ret;
		}
	}
}
