using System;
using System.IO;
using DevUtils.Elas.Tasks.Core.EnvDTE;

namespace DevUtils.Elas.Tasks.Core.IO.Extensions
{
	/// <summary> A file information extensions. </summary>
	public static class FileInfoExtensions
	{
		/// <summary> A FileSystemInfo extension method that ensures that in project. </summary>
		///
		/// <param name="fileInfo"> The fileInfo to act on. </param>
		public static void EnsureInProject(this FileInfo fileInfo)
		{
			DTEFactory.EnsureInProject(fileInfo.FullName);
		}

		/// <summary> A FileInfo extension method that clears the read only described by fileInfo. </summary>
		///
		/// <param name="fileInfo"> The fileInfo to act on. </param>
		public static void DTEClearReadOnly(this FileInfo fileInfo)
		{
			DTEFactory.ClearReadOnly(fileInfo.FullName);
		}

		/// <summary> A FileInfo extension method that quiet delete. </summary>
		///
		/// <param name="fileInfo"> The fileInfo to act on. </param>
		public static void QuietDelete(this FileInfo fileInfo)
		{
			try
			{
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
			}
			catch
			{
				// ignored
			}
		}

		/// <summary> A FileInfo extension method that touches the given file. </summary>
		///
		/// <param name="fileInfo"> The fileInfo to act on. </param>
		public static void Touch(this FileInfo fileInfo)
		{
			fileInfo.LastWriteTimeUtc = DateTime.UtcNow;
		}
	}
}