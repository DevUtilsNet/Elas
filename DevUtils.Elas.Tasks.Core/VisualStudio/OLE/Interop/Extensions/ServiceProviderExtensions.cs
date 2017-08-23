using System;
using System.Runtime.InteropServices;

namespace DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions
{
	static class ServiceProviderExtensions
	{
		public static T QueryService<T>(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider, Type serviceType) where T : class
		{
			var sg = serviceType.GUID;
			var ig = typeof (T).GUID;
			IntPtr ptr;

			var hr = provider.QueryService(ref sg, ref ig, out ptr);
			Marshal.ThrowExceptionForHR(hr);

			try
			{
				var ret = Marshal.GetObjectForIUnknown(ptr);
				return (T)ret;
			}
			finally
			{
				Marshal.Release(ptr);
			}
		}
	}
}
