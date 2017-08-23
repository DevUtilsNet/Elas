using System;

namespace DevUtils.Elas.Tasks.Core
{
	sealed class Math2
	{
		public static void Swap<T>(ref T a, ref T b)
		{
			var c = a;
			a = b;
			b = c;
		}

		public static bool SortPair<T>(ref T lo, ref T hi, Comparison<T> comp)
		{
			if (comp(lo, hi) > 0)
			{
				Swap(ref lo, ref hi);
				return true;
			}
			return false;
		}
	}
}