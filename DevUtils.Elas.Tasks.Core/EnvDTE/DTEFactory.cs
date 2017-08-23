using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using DevUtils.Elas.Tasks.Core.Diagnostics;
using DevUtils.Elas.Tasks.Core.Diagnostics.Extensions;
using DevUtils.Elas.Tasks.Core.EnvDTE.Extensions;
using DevUtils.Elas.Tasks.Core.Runtime.InteropServices.ComTypes.Extensions;
using DevUtils.Elas.Tasks.Core.VisualStudio.OLE.Interop.Extensions;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace DevUtils.Elas.Tasks.Core.EnvDTE
{
	static class DTEFactory
	{
		private static _DTE _dte;

		public static _DTE GetOwnerDTE()
		{
			if (_dte != null)
			{
				ElasTraceSourceCore.Instance.TraceEvent(TraceEventType.Verbose, 1, "Using an existing DET instance.");
				return _dte;
			}

			var table = NativeMethods.GetRunningObjectTable(0);

			ElasTraceSourceCore.Instance.TraceEvent(TraceEventType.Verbose, 2, "Try to find the Visual Studio DTE owner.");

			try
			{
				using (var current = System.Diagnostics.Process.GetCurrentProcess())
				{
					using (var parent = current.GetParentProcess())
					{
						foreach (var item in table.GetMonikers())
						{
							string name;
							var ctx = NativeMethods.CreateBindCtx(0);
							item.GetDisplayName(ctx, null, out name);

							if (name.StartsWith("!VisualStudio.DTE"))
							{
								object obj;
								table.GetObject(item, out obj);

								var dte = obj as _DTE;
								if (dte == null)
								{
									continue;
								}

								var buildStateInProgress = dte.Solution.SolutionBuild.BuildState == vsBuildState.vsBuildStateInProgress;

								var processId = Int32.Parse(name.Split(':')[1]);
								if (processId == current.Id || (parent != null && processId == parent.Id))
								{
#if !DEBUG
									if (buildStateInProgress)
									{
#endif
										_dte = dte;
										return _dte;
#if !DEBUG
									}
#endif
								}
								else if (buildStateInProgress)
								{
									_dte = dte;
								}
							}
						}
					}
				}
			}
			finally
			{
				if (_dte != null)
				{
					ElasTraceSourceCore.Instance.TraceEvent(TraceEventType.Verbose, 3, "Found the Visual Studio DTE owner \"{0}\".", _dte.MainWindow.Caption);
				}
				else
				{
					ElasTraceSourceCore.Instance.TraceEvent(TraceEventType.Verbose, 4, "Not Found");
				}
			}

			return _dte;
		}

		private static void TryCall(Action action)
		{
			for (var i = 0; ; ++i)
			{
				try
				{
					action();
					break;
				}
				catch (COMException e)
				{
					ElasTraceSourceCore.Instance.TraceEvent(TraceEventType.Verbose, 5, e.Message);

					_dte = null;
					// Ignore 5 times
					// A first chance exception of type 'System.Runtime.InteropServices.COMException' occurred in mscorlib.dll
					// Additional information: The message filter indicated that the application is busy. (Exception from HRESULT: 0x8001010A (RPC_E_SERVERCALL_RETRYLATER))
					if (i > 5)
					{
						throw;
					}
					System.Threading.Thread.Sleep(200);
				}
			}
		}

		public static void EnsureInProject(string fullPath, string dependentUpon = null)
		{
			TryCall(
				() => {
					var dte = GetOwnerDTE();
					if (dte == null)
					{
						return;
					}

					var solution = dte.QueryService<IVsSolution2>(typeof (SVsSolution));

					solution.EnsureInSolution(fullPath, dependentUpon);
				});
		}

		public static void ClearReadOnly(string fullPath)
		{
			if (!File.Exists(fullPath))
			{
				return;
			}

			var fa = File.GetAttributes(fullPath);
			if ((fa & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
			{
				return;
			}

			TryCall(
				() => {
					var dte = GetOwnerDTE();
					if (dte == null)
					{
						return;
					}

					var qeqs = dte.QueryService<IVsQueryEditQuerySave2>(typeof (SVsQueryEditQuerySave));

					var fullName = fullPath;
					qeqs.QueryEditFiles2(fullName);
					qeqs.QuerySaveFiles2(fullName);
				});
		}
	}
}
