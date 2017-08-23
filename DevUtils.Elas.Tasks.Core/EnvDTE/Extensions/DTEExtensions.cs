using System;
using DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions;
using EnvDTE;

namespace DevUtils.Elas.Tasks.Core.EnvDTE.Extensions
{
	static class DTEExtensions
	{
		public static T QueryService<T>(this _DTE dte, Type serviceType) where T : class
		{
			var ret = ((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte).QueryService<T>(serviceType);
			return ret;
		}
	}
}