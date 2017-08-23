using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DevUtils.Elas.Tasks.Core.Collections
{
	static class InternalList
	{
		public static T[] CopyToNewArray<T>(T[] _array, int _count, int newCapacity)
		{
			var a = new T[newCapacity];
			if (_array == null)
				return a;
			Array.Copy(_array, a, _count);
			return a;
		}

		public static T[] CopyToNewArray<T>(T[] array)
		{
			return CopyToNewArray(array, array.Length, array.Length);
		}

		public static void Fill<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
				array[i] = value;
		}

		public static void Fill<T>(T[] array, int start, int count, T value)
		{
			if (count > 0)
			{
				// Just for fun, let's unroll the loop
				start--;
				if ((count & 1) != 0)
					array[++start] = value;
				while ((count -= 2) >= 0)
				{
					array[++start] = value;
					array[++start] = value;
				}
			}
		}

		public static int BinarySearch<T>(T[] array, int count, T k, Comparer<T> comp, bool lowerBound)
		{
			int low = 0;
			int high = count - 1;
			int invert = -1;

			while (low <= high)
			{
				int mid = low + ((high - low) >> 1);
				T midk = array[mid];
				int c = comp.Compare(midk, k);
				if (c < 0)
					low = mid + 1;
				else
				{
					high = mid - 1;
					if (c == 0)
					{
						if (lowerBound)
							invert = 0;
						else
							return mid;
					}
				}
			}

			return low ^ invert;
		}

		/// <summary>Performs a binary search with a custom comparison function.</summary>
		/// <param name="_array">Array to search</param>
		/// <param name="_count">Number of elements used in the array</param>
		/// <param name="k">A key to compare with elements of the array</param>
		/// <param name="compare">Lambda function that knows how to compare Ts with 
		/// Ks (T and K can be the same). It is passed a series of elements from 
		/// the array. It must return 0 if the element has the desired value, 1 if 
		/// the supplied element is higher than desired, and -1 if it is lower than 
		/// desired.</param>
		/// <param name="lowerBound">Whether to find the "lower bound" in case there
		/// are duplicates in the list. If duplicates exist of the search key k, the 
		/// lowest index of a matching duplicate is returned. This search mode may be 
		/// slightly slower when a match exists.</param>
		/// <returns>The index of the matching array entry, if found. If no exact
		/// match was found, this method returns the bitwise complement of an
		/// insertion location that would preserve the order.</returns>
		/// <example>
		///     // The first 6 elements are sorted. The seventh is invalid,
		///     // and must be excluded from the binary search.
		///     int[] array = new int[] { 0, 10, 20, 30, 40, 50, -1 };
		///     // The result will be 2, because array[2] == 20.
		///     int a = InternalList.BinarySearch(array, 6, i => i.CompareTo(20));
		///     // The result will be ~2, which equals -3, because index 2 would
		///     // be the correct place to insert 17 to preserve the sort order.
		///     int b = InternalList.BinarySearch(array, 6, i => i.CompareTo(17));
		/// </example>
		public static int BinarySearch<T, K>(T[] _array, int _count, K k, Func<T, K, int> compare, bool lowerBound)
		{
			int low = 0;
			int high = _count - 1;
			int invert = -1;

			while (low <= high)
			{
				int mid = low + ((high - low) >> 1);
				int c = compare(_array[mid], k);
				if (c < 0)
					low = mid + 1;
				else
				{
					high = mid - 1;
					if (c == 0)
					{
						if (lowerBound)
							invert = 0;
						else
							return mid;
					}
				}
			}

			return low ^ invert;
		}

		/// <summary>A binary search function that knows nothing about the list 
		/// being searched.</summary>
		/// <typeparam name="Anything">Any data type relevant to the caller.</typeparam>
		/// <param name="data">State information to be passed to compare()</param>
		/// <param name="count">Number of items in the list being searched</param>
		/// <param name="compare">Comparison method that is given the current index 
		/// to examine and the state parameter "data".</param>
		/// <param name="lowerBound">Whether to find the "lower bound" in case there
		/// are duplicates in the list. If duplicates exist of the search key k 
		/// exist, the lowest index of a matching duplicate is returned. This
		/// search mode may be slightly slower when a match exists.</param>
		/// <returns>The index of the matching index, if found. If no exact
		/// match was found, this method returns the bitwise complement of an
		/// insertion location that would preserve the sort order.</returns>
		public static int BinarySearchByIndex<Anything>(Anything data, int count, Func<int, Anything, int> compare, bool lowerBound)
		{
			var low = 0;
			var high = count - 1;
			var invert = -1;

			while (low <= high)
			{
				int mid = low + ((high - low) >> 1);
				int c = compare(mid, data);
				if (c < 0)
					low = mid + 1;
				else
				{
					high = mid - 1;
					if (c == 0)
					{
						if (lowerBound)
							invert = 0;
						else
							return mid;
					}
				}
			}

			return low ^ invert;
		}

		/// <summary>As an alternative to the typical enlarging pattern of doubling
		/// the array size when it overflows, this function proposes a 75% size
		/// increase instead (100% when the array is small), while ensuring that
		/// the array length stays even.</summary>
		/// <remarks>
		/// With a seed of 0, 2, or 4: 0, 2, 4, 8, 16, 30, 54, 96, 170, 298, 522...<br/>
		/// With a seed of 1: 1, 2, 4, 8, 16, 30, 54, 96, 170, 298, 522...<br/>
		/// With a seed of 3: 3, 6, 12, 22, 40, 72, 128, 226, 396...<br/>
		/// With a seed of 5: 5, 10, 18, 32, 58, 102, 180, 316, 554...<br/>
		/// With a seed of 7: 7, 14, 26, 46, 82, 144, 254, 446, 782...
		/// <para/>
		/// 75% size increases require 23.9% more allocations than size doubling
		/// (1.75 to the 1.239th power is about 2.0), but memory utilization is
		/// increased. With size doubling, the average list uses 2/3 of its 
		/// entries, but with this resizing pattern, the average list uses 72.72%
		/// of its entries. The average size of a list is 8.3% lower. Originally
		/// I used 50% size increases, but they required 71% more allocations, 
		/// which seemed like too much.
		/// </remarks>
		public static int NextLargerSize(int than)
		{
			return ((than << 1) - (than >> 2) + 2) & ~1;
		}
		/// <summary>Same as <see cref="NextLargerSize(int)"/>, but allows you to 
		/// specify a capacity limit, to avoid wasting memory when a collection has 
		/// a known maximum size.</summary>
		/// <param name="than">Return value will be larger than this number.</param>
		/// <param name="capacityLimit">Maximum value to return. This parameter is
		/// ignored if it than >= capacityLimit.</param>
		/// <returns>Produces the same result as <see cref="NextLargerSize(int)"/>
		/// unless the return value would be near capacityLimit (and capacityLimit
		/// > than). If the return value would be more than capacityLimit, 
		/// capacityLimit is returned instead. If the return value would be slightly
		/// less than capacityLimit (within 20%) then capacityLimit is returned, 
		/// to ensure that another reallocation will not be required later.</returns>
		public static int NextLargerSize(int than, int capacityLimit)
		{
			int larger = NextLargerSize(than);
			if (larger + (larger >> 2) > capacityLimit && than < capacityLimit)
				return capacityLimit;
			return larger;
		}

		public static T[] Insert<T>(int index, T item, T[] array, int count)
		{
			Debug.Assert((uint)index <= (uint)count);
			if (count == array.Length)
			{
				var newCap = NextLargerSize(array.Length);
				array = CopyToNewArray(array, count, newCap);
			}
			if (count - index > 0)
				Array.Copy(array, index, array, index + 1, count - index);
			//for (int i = count; i > index; i--)
			//	array[i] = array[i - 1];
			array[index] = item;
			return array;
		}

		public static T[] InsertRangeHelper<T>(int index, int spaceNeeded, T[] array, int count)
		{
			Debug.Assert((uint)index <= (uint)count);
			array = AutoRaiseCapacity(array, count, spaceNeeded, int.MaxValue);
			if (count - index > 0)
				Array.Copy(array, index, array, index + spaceNeeded, count - index);
			//for (int i = count; i > index; i--)
			//	array[i + spaceNeeded - 1] = array[i - 1];
			return array;
		}

		public static T[] AutoRaiseCapacity<T>(T[] array, int count, int more, int capacityLimit)
		{
			if (count + more > array.Length)
			{
				var newCapacity = NextLargerSize(count + more - 1, capacityLimit);
				return CopyToNewArray(array, count, newCapacity);
			}
			return array;
		}

		public static int RemoveAt<T>(int index, T[] array, int count)
		{
			Debug.Assert((uint)index < (uint)count);
			Array.Copy(array, index + 1, array, index, count - index - 1);
			//for (int i = index; i + 1 < count; i++)
			//	array[i] = array[i + 1];
			array[count - 1] = default(T);
			return count - 1;
		}

		public static int RemoveAt<T>(int index, int removeCount, T[] array, int count)
		{
			Debug.Assert((uint)index <= (uint)count);
			Debug.Assert((uint)(index + removeCount) <= (uint)count);
			Debug.Assert(removeCount >= 0);
			if (removeCount > 0)
			{
				Array.Copy(array, index + removeCount, array, index, count - index - removeCount);
				//for (int i = index; i + removeCount < count; i++)
				//	array[i] = array[i + removeCount];
				for (int i = count - removeCount; i < count; i++)
					array[i] = default(T);
				return count - removeCount;
			}
			return count;
		}

		public static void Move<T>(T[] array, int from, int to)
		{
			T saved = array[from];
			if (to < from)
			{
				//Array.Copy(array, to, array, to + 1, from - to);
				for (int i = from; i > to; i--)
					array[i] = array[i - 1];
				array[to] = saved;
			}
			else if (from < to)
			{
				//Array.Copy(array, from + 1, array, from, to - from);
				for (int i = from; i < to; i++)
					array[i] = array[i + 1];
				array[to] = saved;
			}
		}

		internal const int QuickSortThreshold = 9;
		internal const int QuickSortMedianThreshold = 15;

		/// <summary>Performs a quicksort using a Comparison function.</summary>
		/// <remarks>
		/// Normally one uses Array.Sort for sorting arrays.
		/// This method exists because there is no Array.Sort overload that
		/// accepts both a Comparison and a range (index, count), nor does the
		/// .NET framework provide access to its internal adapter that converts 
		/// Comparison to IComparer.
		/// <para/>
		/// This quicksort algorithm uses a best-of-three pivot so that it remains
		/// performant (fast) if the input is already sorted. It is designed to 
		/// perform reasonably well in case the data contains many duplicates (not
		/// verified). It is also designed to avoid using excessive stack space if 
		/// a worst-case input occurs that requires O(N^2) time.
		/// </remarks>
		public static void Sort<T>(T[] array, int index, int count, Comparison<T> comp)
		{
			Debug.Assert((uint)index <= (uint)array.Length);
			Debug.Assert((uint)count <= (uint)array.Length - (uint)index);

			for (; ; )
			{
				if (count < QuickSortThreshold)
				{
					if (count <= 2)
					{
						if (count == 2)
							Math2.SortPair(ref array[index], ref array[index + 1], comp);
					}
					else
					{
						InsertionSort(array, index, count, comp);
					}
					return;
				}

				int iPivot = PickPivot(array, index, count, comp);

				int iBegin = index;
				// Swap the pivot to the beginning of the range
				T pivot = array[iPivot];
				if (iBegin != iPivot)
					Math2.Swap(ref array[iBegin], ref array[iPivot]);

				int i = iBegin + 1;
				int iOut = iBegin;
				int iStop = index + count;
				int leftSize = 0; // size of left partition

				// Quick sort pass
				do
				{
					int order = comp(array[i], pivot);
					if (order < 0 || (order == 0 && leftSize < (count >> 1)))
					{
						++iOut;
						++leftSize;
						if (i != iOut)
							Math2.Swap(ref array[i], ref array[iOut]);
					}
				} while (++i != iStop);

				// Finally, put the pivot element in the middle (at iOut)
				Math2.Swap(ref array[iBegin], ref array[iOut]);

				// Now we need to sort the left and right sub-partitions. Use a 
				// recursive call only to sort the smaller partition, in order to 
				// guarantee O(log N) stack space usage.
				int rightSize = count - 1 - leftSize;
				if (leftSize < rightSize)
				{
					// Recursively sort the left partition; iteratively sort the right
					Sort(array, index, leftSize, comp);
					index += leftSize + 1;
					count = rightSize;
				}
				else
				{	// Iteratively sort the left partition; recursively sort the right
					count = leftSize;
					Sort(array, index + leftSize + 1, rightSize, comp);
				}
			}
		}

		internal static int PickPivot<T>(IList<T> list, int index, int count, Comparison<T> comp)
		{
			// Choose the median of the first, last and middle item
			var iPivot0 = index;
			int iPivot1 = index + (count >> 1);
			int iPivot2 = index + count - 1;
			if (comp(list[iPivot0], list[iPivot1]) > 0)
				Math2.Swap(ref iPivot0, ref iPivot1);
			if (comp(list[iPivot1], list[iPivot2]) > 0)
			{
				iPivot1 = iPivot2;
				if (comp(list[iPivot0], list[iPivot1]) > 0)
					iPivot1 = iPivot0;
			}
			return iPivot1;
		}

		/// <summary>Performs an insertion sort.</summary>
		/// <remarks>The insertion sort is a stable sort algorithm that is slow in 
		/// general (O(N^2)). It should be used only when (a) the list to be sorted
		/// is short (less than about 20 elements) or (b) the list is very nearly
		/// sorted already.</remarks>
		public static void InsertionSort<T>(T[] array, int index, int count, Comparison<T> comp)
		{
			for (int i = index + 1; i < index + count; i++)
			{
				int j = i;
				do
					if (!Math2.SortPair(ref array[j - 1], ref array[j], comp))
						break;
				while (--j > index);
			}
		}

		//public static bool AllEqual<T>(this InternalList<T> a, InternalList<T> b) where T : IEquatable<T>
		//{
		//	return a.Count == b.Count && AllEqual(a.InternalArray, b.InternalArray, a.Count);
		//}
		public static bool AllEqual<T>(T[] a, T[] b, int count) where T : IEquatable<T>
		{
			for (int i = 0; i < count; i++)
				if (!a[i].Equals(b[i]))
					return false;
			return true;
		}
	}
}
