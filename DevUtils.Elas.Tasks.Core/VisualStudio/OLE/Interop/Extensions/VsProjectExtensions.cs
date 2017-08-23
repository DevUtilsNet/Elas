using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions
{
	enum Vsitemid : uint
	{
		Nil = 4294967295u,
		Root = 4294967294u,
		Selection = 4294967293u
	}

	static class VsProjectExtensions
	{
		public static bool IsDocumentInProject2(this IVsProject project, string documentFullPath)
		{
			int found;
			uint itemId;
			var priority = new VSDOCUMENTPRIORITY[1];
			var hr = project.IsDocumentInProject(documentFullPath, out found, priority, out itemId);
			Marshal.ThrowExceptionForHR(hr);

			return found != 0;
		}

		public static Project GetDTEProject(this IVsProject project)
		{
			var vsHierarchy = project as IVsHierarchy;
			if (vsHierarchy == null)
			{
				throw new ArgumentException("IVsProject is not a project.");
			}

			object ret;
			var hr = vsHierarchy.GetProperty((uint)Vsitemid.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out ret);
			Marshal.ThrowExceptionForHR(hr);
			return ret as Project;
		}
	}
}