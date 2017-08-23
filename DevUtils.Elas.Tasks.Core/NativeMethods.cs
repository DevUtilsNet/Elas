using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace DevUtils.Elas.Tasks.Core
{
	static class NativeMethods
	{
		[DllImport("ole32.dll", PreserveSig = false)]
		public static extern IBindCtx CreateBindCtx(int reserved);

		[DllImport("ole32.dll", PreserveSig = false)]
		public static extern IRunningObjectTable GetRunningObjectTable(int reserved);

		[DllImport("ntdll.dll")]
		private static extern int NtQueryInformationProcess(
			IntPtr processHandle, 
			int processInformationClass, 
			ref ParentProcessUtilities processInformation,
			int processInformationLength,
			out int returnLength);


		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		private static extern bool PathRelativePathTo(
				 [Out] StringBuilder pszPath,
				 [In] string pszFrom,
				 [In] FileAttributes dwAttrFrom,
				 [In] string pszTo,
				 [In] FileAttributes dwAttrTo
		);

		[DllImport("ntdll.dll")]
		private static extern int RtlNtStatusToDosError(int status);

		private static void ThrowExceptionForNTSTATUS(int status)
		{
			if (status != 0)
			{
				var hr = RtlNtStatusToDosError(status) | unchecked((int)0x80070000);
				Marshal.ThrowExceptionForHR(hr);
			}
		}

		/// <summary>
		/// A utility class to determine a process parent.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ParentProcessUtilities
		{
			// These members must match PROCESS_BASIC_INFORMATION
			internal IntPtr Reserved1;
			internal IntPtr PebBaseAddress;
			internal IntPtr Reserved2_0;
			internal IntPtr Reserved2_1;
			internal IntPtr UniqueProcessId;
			internal IntPtr InheritedFromUniqueProcessId;
		}

		public static ParentProcessUtilities QueryInformationProcess(IntPtr processHandle, int processInformationClass)
		{
			var ret = new ParentProcessUtilities();

			int returnLength;
			var status = NtQueryInformationProcess(processHandle, processInformationClass, ref ret, Marshal.SizeOf(ret), out returnLength);

			ThrowExceptionForNTSTATUS(status);

			return ret;
		}

		public static string GetRelativePath(string from, FileAttributes fromAttr, string to, FileAttributes toAttr)
		{
			var path = new StringBuilder(260); // MAX_PATH

			if (!PathRelativePathTo(path, from, fromAttr, to, toAttr))
			{
				throw new ArgumentException("The paths do not have to be fully qualified, but they must have a common prefix");
			}

			return path.ToString();
		}
	}
}
