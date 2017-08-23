using System;
using System.IO;
using DevUtils.Elas.Tasks.Core.EnvDTE;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DevUtils.Elas.Tasks.Core.Build.Framework.Extensions
{
	/// <summary> A task item extensions. </summary>
	public static class TaskItemExtensions
	{
		/// <summary> An ITaskItem extension method that request metadata. </summary>
		///
		/// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
		///
		/// <param name="taskItem">		  The taskItem to act on. </param>
		/// <param name="metadataName"> Name of the metadata. </param>
		///
		/// <returns> A string. </returns>
		public static string RequestMetadata(this ITaskItem taskItem, string metadataName)
		{
			var ret = taskItem.GetMetadata(metadataName);
			if (string.IsNullOrEmpty(ret))
			{
				throw new Exception(string.Format("Item \"{0}\" should have \"{1}\" metadata.", taskItem, metadataName));
			}
			return ret;
		}

		/// <summary> An ITaskItem extension method that ensures that in project. </summary>
		///
		/// <param name="taskItem"> The taskItem to act on. </param>
		public static void EnsureInProject(this ITaskItem taskItem)
		{
			DTEFactory.EnsureInProject(taskItem.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath), taskItem.GetMetadata("DependentUpon"));
		}

		/// <summary> An ITaskItem extension method that gets relative path. </summary>
		///
		/// <param name="taskItem"> The taskItem to act on. </param>
		/// <param name="to">			  to. </param>
		/// <param name="isFile">	  (Optional) true if this object is file. </param>
		/// <param name="toIsFile"> (Optional) true if to is file. </param>
		///
		/// <returns> The relative path. </returns>
		public static string GetRelativePath(this ITaskItem taskItem, string to, bool isFile = true, bool toIsFile = true)
		{
			var ret = NativeMethods.GetRelativePath(
				taskItem.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath),
				isFile ? FileAttributes.Normal : FileAttributes.Directory, Path.GetFullPath(to), 
				toIsFile ? FileAttributes.Normal : FileAttributes.Directory);

			return ret;
		}

		/// <summary> An ITaskItem extension method that creates relative item. </summary>
		///
		/// <param name="taskItem"> The taskItem to act on. </param>
		/// <param name="newPath">  Full pathname of the new file. </param>
		///
		/// <returns> The new relative item. </returns>
		public static ITaskItem CreateRelativeItem(this ITaskItem taskItem, string newPath)
		{
			var ret = new TaskItem(taskItem)
			{
				ItemSpec = newPath
			};

			ret.SetMetadata("ElasSourceItemSpec", taskItem.ItemSpec);
			var depends = taskItem.GetMetadata("DependentUpon");
			if (!string.IsNullOrEmpty(depends))
			{
				var dependsRelativePath = ret.GetRelativePath(Path.Combine(
					Path.GetDirectoryName(taskItem.RequestMetadata(MSBuildWellKnownItemMetadates.FullPath)), depends));

				ret.SetMetadata("DependentUpon", dependsRelativePath);
			}
			return ret;
		}
	}
}