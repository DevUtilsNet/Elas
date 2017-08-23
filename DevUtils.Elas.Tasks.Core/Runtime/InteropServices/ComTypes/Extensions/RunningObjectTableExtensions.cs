using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace DevUtils.Elas.Tasks.Core.Runtime.InteropServices.ComTypes.Extensions
{
	static class RunningObjectTableExtensions
	{
		public static IEnumerable<IMoniker> GetMonikers(this IRunningObjectTable objectTable)
		{
			IEnumMoniker enumMoniker;
			objectTable.EnumRunning(out enumMoniker);
			enumMoniker.Reset();

			for (var monikers = new IMoniker[1]; enumMoniker.Next(monikers.Length, monikers, IntPtr.Zero) == 0; )
			{
				yield return monikers[0];
			}
		}
	}
}