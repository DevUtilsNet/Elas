using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions
{
	static class VsQueryEditQuerySave2Extensions
	{
		public static void QueryEditFiles2(this IVsQueryEditQuerySave2 editQuerySave, params string[] files)
		{
			uint editResult;
			uint editResultFlags;

			var null1 = new uint[0];
			var null2 = new VSQEQS_FILE_ATTRIBUTE_DATA[0];

			var hr = editQuerySave.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_AllowInMemoryEdits, files.Length, files, null1, null2, out editResult, out editResultFlags);
			Marshal.ThrowExceptionForHR(hr);

			if (((tagVSQueryEditResult)editResult) != tagVSQueryEditResult.QER_EditOK)
			{
				throw new OperationCanceledException();
			}
		}

		public static void QuerySaveFiles2(this IVsQueryEditQuerySave2 editQuerySave, params string[] files)
		{
			uint editResult;
			var null1 = new uint[0];
			var null2 = new VSQEQS_FILE_ATTRIBUTE_DATA[0];

			var hr = editQuerySave.QuerySaveFiles(0, files.Length, files, null1, null2, out editResult);
			Marshal.ThrowExceptionForHR(hr);

			if (((tagVSQuerySaveResult)editResult) != tagVSQuerySaveResult.QSR_SaveOK)
			{
				throw new OperationCanceledException();
			}
		}
	}
}