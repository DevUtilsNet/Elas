using System;
using System.Collections.Generic;

namespace DevUtils.Elas.Tasks.Core.Collections.Extensions
{
	static class ListExtensions
	{
		public static void Sort<T>(this IList<T> list)
		{
			Sort(list, Comparer<T>.Default.Compare);
		}

		public static void Sort<T>(this IList<T> list, Comparison<T> comp)
		{
			Sort(list, 0, list.Count, comp, null);
		}

		private static void Sort<T>(this IList<T> list, int index, int count, Comparison<T> comp,
																int[] indexes, int quickSelectElems = int.MaxValue)
		{
			SortCore(list, index, count, comp, indexes, quickSelectElems);
		}

		/// <summary>Swaps list[i] with list[j].</summary>
		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			var tmp = list[i];
			list[i] = list[j];
			list[j] = tmp;
		}

		/// <summary>Performs an insertion sort.</summary>
		/// <remarks>The insertion sort is a stable sort algorithm that is slow in 
		/// general (O(N^2)). It should be used only when (a) the list to be sorted
		/// is short (less than 10-20 elements) or (b) the list is very nearly
		/// sorted already.</remarks>
		public static void InsertionSort<T>(this IList<T> array, int index, int count, Comparison<T> comp)
		{
			for (var i = index + 1; i < index + count; i++)
			{
				var j = i;
				do
				{
					if (!SortPair(array, j - 1, j, comp))
					{
						break;
					}
				}
				while (--j > index);
			}
		}

		/// <summary>Sorts two items to ensure that list[i] is less than list[j].</summary>
		/// <returns>True if the array elements were swapped, false if not.</returns>
		public static bool SortPair<T>(this IList<T> list, int i, int j, Comparison<T> comp)
		{
			if (i != j && comp(list[i], list[j]) > 0)
			{
				Swap(list, i, j);
				return true;
			}
			return false;
		}

		public static int BinarySearch<T>(this IReadOnlyList<T> list, T find, Comparison<T> compare)
		{
			int low = 0, high = list.Count - 1;
			var invert = -1;
			while (low <= high)
			{
				var mid = low + ((high - low) >> 1);
				var c = compare(list[mid], find);
				if (c < 0)
				{
					low = mid + 1;
				}
				else
				{
					high = mid - 1;
					if (c == 0)
					{
						invert = 0;
					}
				}
			}
			return low ^ invert;
		}

		// Used by Sort, StableSort, SortLowestK, SortLowestKStable.
		private static void SortCore<T>(this IList<T> list, int index, int count, Comparison<T> comp,
																		int[] indexes, int quickSelectElems)
		{
			// This code duplicates the code in InternalList.Sort(), except
			// that it also supports stable sorting (indexes parameter) and
			// quickselect (sorting the first 'quickSelectElems' elements). This 
			// version is slower; two versions exist so that array sorting can 
			// be done faster.
			if (quickSelectElems <= 0)
				return;

			for (; ; )
			{
				if (count < InternalList.QuickSortThreshold)
				{
					if (count <= 2)
					{
						if (count == 2)
						{
							int c = comp(list[index], list[index + 1]);
							if (c > 0 || (c == 0 && indexes != null && indexes[index] > indexes[index + 1]))
								Swap(list, index, index + 1);
						}
						return;
					}

					if (indexes == null)
					{
						InsertionSort(list, index, count, comp);
						return;
					}
				}

				// TODO: fix slug: PickPivot does not use 'indexes'. Makes stable sort slower if many duplicate items.
				var iPivot = InternalList.PickPivot(list, index, count, comp);

				int iBegin = index;
				// Swap the pivot to the beginning of the range
				T pivot = list[iPivot];
				if (iBegin != iPivot)
				{
					Swap(list, iBegin, iPivot);
					if (indexes != null)
						Math2.Swap(ref indexes[iPivot], ref indexes[iBegin]);
				}

				int i = iBegin + 1;
				int iOut = iBegin;
				int iStop = index + count;
				int leftSize = 0; // size of left partition

				// Quick sort pass
				do
				{
					int order = comp(list[i], pivot);
					if (order > 0)
						continue;
					if (order == 0)
					{
						if (indexes != null)
						{
							if (indexes[i] > indexes[iBegin])
								continue;
						}
						else if (leftSize < (count >> 1))
							continue;
					}

					++iOut;
					++leftSize;
					if (i != iOut)
					{
						Swap(list, i, iOut);
						if (indexes != null)
							Math2.Swap(ref indexes[i], ref indexes[iOut]);
					}
				} while (++i != iStop);

				// Finally, put the pivot element in the middle (at iOut)
				Swap(list, iBegin, iOut);
				if (indexes != null)
					Math2.Swap(ref indexes[iBegin], ref indexes[iOut]);

				// Now we need to sort the left and right sub-partitions. Use a 
				// recursive call only to sort the smaller partition, in order to 
				// guarantee O(log N) stack space usage.
				int rightSize = count - 1 - leftSize;
				if (leftSize < rightSize)
				{
					// Recursively sort the left partition; iteratively sort the right
					SortCore(list, index, leftSize, comp, indexes, quickSelectElems);
					index += leftSize + 1;
					count = rightSize;
					if ((quickSelectElems -= leftSize + 1) <= 0)
						break;
				}
				else
				{	// Iteratively sort the left partition; recursively sort the right
					count = leftSize;
					SortCore(list, index + leftSize + 1, rightSize, comp, indexes, quickSelectElems - (leftSize + 1));
				}
			}
		}
	}
}
