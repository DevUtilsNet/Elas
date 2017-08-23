using System.Collections;
using System.Linq;

namespace DevUtils.Elas.Tasks.Core.Collections.Extensions
{
	/// <summary> An extensions. </summary>
	public static class EnumerableExtensions
	{
		/// <summary> An IEnumerable extension method that empty if null. </summary>
		/// <param name="enumerable"> The enumerable to act on. </param>
		/// <returns> An IEnumerable. </returns>
		public static IEnumerable EmptyIfNull(this IEnumerable enumerable)
		{
			return enumerable ?? Enumerable.Empty<int>();
		}

	}
}
