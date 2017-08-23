using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DevUtils.Elas.Tasks.Core.EnvDTE.Extensions;
using Microsoft.VisualStudio.Shell.Interop;

namespace DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions
{
	static class VsSolution2Extensions
	{
		public static IEnumerable<IVsHierarchy> GetHierarchy(this IVsSolution2 solution, __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_ALLPROJECTS)
		{
			var empty = Guid.Empty;
			IEnumHierarchies enumHierarchies;
			var hr = solution.GetProjectEnum((uint)flags, ref empty, out enumHierarchies);
			Marshal.ThrowExceptionForHR(hr);

			uint fetched;
			for (var item = new IVsHierarchy[10]; enumHierarchies.Next((uint)item.Length, item, out fetched) >= 0 && fetched > 0; )
			{
				for (var i = 0; i < fetched; ++i)
				{
					var vsHierarchy = item[i];
					yield return vsHierarchy;
				}
			}
		}

		public static IVsProject GetProjectByFileFullPath(this IVsSolution2 solution, string fullPath)
		{
			var ret = solution.GetHierarchy().OfType<IVsProject>().FirstOrDefault(project => project.IsDocumentInProject2(fullPath));
			return ret;
		}

		public static IVsProject FindRelativeProject(this IVsSolution2 solution, string fullPath)
		{
			foreach (var item in solution.GetHierarchy(__VSENUMPROJFLAGS.EPF_ALLINSOLUTION))
			{
				object ret;
				var hr = item.GetProperty((uint)Vsitemid.Root, (int)__VSHPROPID.VSHPROPID_ProjectDir, out ret);
				if (hr != 0)
				{
					continue;
				}

				var path = string.Empty;

				try
				{
					path = NativeMethods.GetRelativePath((string)ret, FileAttributes.Directory, fullPath, FileAttributes.Normal);
				}
				// ReSharper disable once EmptyGeneralCatchClause
				catch
				{
				}

				if (path.StartsWith(".\\"))
				{
					return (IVsProject) item;
				}
			}

			return null;
		}

		public static void EnsureInSolution(this IVsSolution2 solution, string fullPath, string dependentUpon = null)
		{
			var vsProject = solution.FindRelativeProject(fullPath);
			if (vsProject != null)
			{
				var project = vsProject.GetDTEProject();

				project.AddToProject(fullPath, dependentUpon);
			}
		}
	}
}