using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevUtils.Elas.Tasks.Core.Collections;

namespace DevUtils.Elas.Tasks.Core.Loyc.Collections
{
	/// <summary>A compact auto-enlarging array structure that is intended to be 
	/// used within other data structures. It should only be used internally in
	/// "private" or "protected" members of low-level code.
	/// </summary>
	/// InternalList is a struct, not a class, in order to save memory; and for 
	/// maximum performance, it asserts rather than throwing an exception 
	/// when an incorrect array index is used. Besides that, it has an 
	/// InternalArray property that provides access to the internal array. 
	/// For all these reasons one should not expose it in a public API, and 
	/// it should only be used when performance trumps all other concerns.
	/// <para/>
	/// Passing this structure by value is dangerous because changes to a copy 
	/// of the structure may or may not be reflected in the original list. It's
	/// best not to pass it around at all, but if you must pass it, pass it by
	/// reference.
	/// <para/>
	/// Also, do not use the default contructor. Always specify an initial 
	/// capacity or copy InternalList.Empty so that _array gets a value. 
	/// This is required because methods such as Add(), Insert() and Resize() 
	/// assume _array is not null.
	/// <para/>
	/// InternalList has one nice thing that List(of T) lacks: a <see cref="Resize(int)"/>
	/// method and an equivalent Count setter. Which dork at Microsoft decided no 
	/// one should be allowed to set the list length directly? This type also 
	/// provides a handy <see cref="Last"/> property and a <see cref="Pop"/> 
	/// method to respectively get or remove the last item.
	/// <para/>
	/// Finally, alongside InternalList(T), the static class InternalList comes 
	/// with some static methods (CopyToNewArray, Insert, RemoveAt, Move) to help
	/// manage raw arrays. You might want to use these in a data structure 
	/// implementation even if you choose not to use InternalList(T) instances.
	sealed class InternalList<T> : IListSource<T>, ICloneable
	{
		public static readonly T[] EmptyArray = new T[0];
		public static readonly InternalList<T> Empty = new InternalList<T>(0);
		private T[] _array;
		private int _count;

		public InternalList(int capacity)
		{
			_count = 0;
			_array = capacity != 0 ? new T[capacity] : EmptyArray;
		}
		public InternalList(T[] array, int count)
		{
			_array = array;
			_count = count;
		}
		public InternalList(IEnumerable<T> items) : this(items.GetEnumerator()) { }
		public InternalList(IEnumerator<T> items)
		{
			_count = 0;
			_array = EmptyArray;
			AddRange(items);
		}

		public int Count
		{
			[DebuggerStepThrough]
			get { return _count; }
			set { Resize(value); }
		}

		public bool IsEmpty
		{
			[DebuggerStepThrough]
			get { return _count == 0; }
		}

		/// <summary>Gets or sets the array length.</summary>
		/// <remarks>Changing this property requires O(Count) time and temporary 
		/// space. Attempting to set the capacity lower than Count has no effect.
		/// </remarks>
		public int Capacity
		{
			[DebuggerStepThrough]
			get { return _array.Length; }
			set
			{
				if (_array.Length != value && value >= _count)
					_array = InternalList.CopyToNewArray(_array, _count, value);
			}
		}


		public void AutoRaiseCapacity(int more, int capacityLimit)
		{
			_array = InternalList.AutoRaiseCapacity(_array, _count, more, capacityLimit);
		}

		private void IncreaseCapacity()
		{
			// 4, 8, 14, 22, 34, 52, 80...
			Capacity = InternalList.NextLargerSize(_array.Length);
		}

		/// <summary>Makes the list larger or smaller, depending on whether 
		/// <c>newSize</c> is larger or smaller than <see cref="Count"/>.</summary>
		/// <param name="newSize">New value of <see cref="Count"/>. If the Count
		/// increases, copies of default(T) are added to the end of the the list; 
		/// otherwise items are removed from the end of the list.</param>
		public void Resize(int newSize) { Resize(newSize, true); }
		/// <inheritdoc cref="Resize(int)"/>
		public void Resize(int newSize, bool allowReduceCapacity)
		{
			if (newSize > _count)
			{
				if (newSize > _array.Length)
				{
					if (newSize <= _array.Length + (_array.Length >> 2))
					{
						IncreaseCapacity();
						Debug.Assert(Capacity > newSize);
					}
					else
						Capacity = newSize;
				}
				_count = newSize;
			}
			else if (newSize < _count)
			{
				if (allowReduceCapacity && newSize < (_array.Length >> 2))
				{
					_count = newSize;
					Capacity = newSize;
				}
				else
				{
					for (int i = newSize; i < _count; i++)
						_array[i] = default(T);
					_count = newSize;
				}
			}
		}

		public void Add(T item)
		{
			if (_count == _array.Length)
				IncreaseCapacity();
			_array[_count++] = item;
		}
		public void AddRange(IEnumerator<T> items)
		{
			while (items.MoveNext())
				Add(items.Current);
		}
		public void Insert(int index, T item)
		{
			_array = InternalList.Insert(index, item, _array, _count);
			_count++;
		}
		//public void InsertRange(int index, ICollectionAndReadOnly<T> items)
		//{
		//	InsertRange(index, items, ((IReadOnlyCollection<T>)items).Count);
		//}
		public void InsertRange(int index, IReadOnlyCollection<T> items)
		{
			InsertRange(index, items, items.Count);
		}
		public void InsertRange(int index, ICollection<T> items)
		{
			InsertRange(index, items, items.Count);
		}
		private void InsertRange(int index, IEnumerable<T> items, int count)
		{
			_array = InternalList.InsertRangeHelper(index, count, _array, _count);
			_count += count;

			int stop = index + count;
			foreach (var item in items)
			{
				if (index >= stop)
					InsertRangeSizeMismatch();
				_array[index++] = item;
			}
			if (index < stop)
				InsertRangeSizeMismatch();
		}
		public void InsertRange(int index, IEnumerable<T> e)
		{
			var s = e as IReadOnlyCollection<T>;
			if (s != null)
				InsertRange(index, s);
			var c = e as ICollection<T>;
			if (c != null)
				InsertRange(index, c);
			else
				InsertRange(index, (ICollection<T>)new List<T>(e));
		}

		public void AddRange(IReadOnlyCollection<T> items)
		{
			InsertRange(_count, items);
		}
		public void AddRange(ICollection<T> items)
		{
			InsertRange(_count, items);
		}
		public void AddRange(IEnumerable<T> e)
		{
			foreach (var item in e)
			{
				Insert(_count, item);
			}
		}
		//public void AddRange(ICollectionAndReadOnly<T> items)
		//{
		//	InsertRange(_count, (IReadOnlyCollection<T>)items);
		//}

		private void InsertRangeSizeMismatch()
		{
			throw new ArgumentException("InsertRange: Input collection's Count is different from the number of items enumerated");
		}

		/// <summary>Clears the list and frees the memory used by the list. Can 
		/// also be used to initialize a list whose constructor was never called.</summary>
		public void Clear()
		{
			_count = 0;
			_array = EmptyArray;
		}

		public void RemoveAt(int index)
		{
			_count = InternalList.RemoveAt(index, _array, _count);
		}
		public void RemoveRange(int index, int count)
		{
			_count = InternalList.RemoveAt(index, count, _array, _count);
		}

		public T this[int index]
		{
			[DebuggerStepThrough]
			get
			{
				Debug.Assert((uint)index < (uint)_count);
				return _array[index];
			}
			set
			{
				Debug.Assert((uint)index < (uint)_count);
				_array[index] = value;
			}
		}

		public T First
		{
			get { return _array[0]; }
			set { _array[0] = value; }
		}
		public T Last
		{
			get
			{
				return _array[_count - 1];
			}
			set
			{
				_array[_count - 1] = value;
			}
		}
		public void Pop()
		{
			_array[_count - 1] = default(T);
			_count--;
		}

		/// <summary>Makes a copy of the list with the same capacity</summary>
		public object Clone()
		{
			return new InternalList<T>(InternalList.CopyToNewArray(_array, _count, _array.Length), _count);
		}
		/// <summary>Makes a copy of the list with Capacity = Count</summary>
		public InternalList<T> CloneAndTrim()
		{
			return new InternalList<T>(InternalList.CopyToNewArray(_array, _count, _count), _count);
		}
		/// <summary>Makes a copy of the list, as an array</summary>
		public T[] ToArray()
		{
			return InternalList.CopyToNewArray(_array, _count, _count);
		}

		public int BinarySearch(T lookFor)
		{
			return InternalList.BinarySearch(_array, _count, lookFor, Comparer<T>.Default, false);
		}
		public int BinarySearch(T lookFor, Comparer<T> comp)
		{
			return InternalList.BinarySearch(_array, _count, lookFor, comp, false);
		}
		public int BinarySearch(T lookFor, Comparer<T> comp, bool lowerBound)
		{
			return InternalList.BinarySearch(_array, _count, lookFor, comp, lowerBound);
		}
		public int BinarySearch<K>(K lookFor, Func<T, K, int> func, bool lowerBound)
		{
			return InternalList.BinarySearch(_array, _count, lookFor, func, lowerBound);
		}

		/// <summary>Slides the array entry at [from] forward or backward in the
		/// list, until it reaches [to].</summary>
		/// <remarks>
		/// For example, if a list of integers is [0, 1, 2, 3, 4, 5] then Move(4,1)
		/// produces the following result: [0, 4, 1, 2, 3, 5].
		/// </remarks>
		public void Move(int from, int to)
		{
			Debug.Assert((uint)from < (uint)_count);
			Debug.Assert((uint)to < (uint)_count);
			InternalList.Move(_array, from, to);
		}

		#region Boilerplate

		public int IndexOf(T item) { return IndexOf(item, 0); }
		public int IndexOf(T item, int index)
		{
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for (; index < Count; index++)
				if (comparer.Equals(this[index], item))
					return index;
			return -1;
		}
		public bool Contains(T item)
		{
			return IndexOf(item) != -1;
		}
		public void CopyTo(T[] array, int arrayIndex)
		{
			Array.Copy(_array, 0, array, arrayIndex, _count);
		}
		public bool IsReadOnly
		{
			get { return false; }
		}
		public bool Remove(T item)
		{
			var i = IndexOf(item);
			if (i == -1)
			{
				return false;
			}
			RemoveAt(i);
			return true;
		}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		//{
		//	return GetEnumerator();
		//}

		public IEnumerator<T> GetEnumerator()
		{
			for (var i = 0; i < Count; i++)
			{
				yield return this[i];
			}
		}
		public T[] InternalArray
		{
			[DebuggerStepThrough]
			get { return _array; }
		}

		#endregion

		//public Iterator<T> GetIterator(int start, int subcount)
		//{
		//    Debug.Assert(subcount >= 0 && (uint)start <= (uint)_count);
		//    if (subcount > _count - start)
		//        subcount = _count - start;
		//    return InternalList.GetIterator(_array, start, subcount);
		//}
		public T TryGet(int index, out bool fail)
		{
			if ((uint)index < (uint)_count)
			{
				fail = false;
				return _array[index];
			}
			fail = true;
			return default(T);
		}

		public void Sort(Comparison<T> comp) { Sort(0, Count, comp); }
		public void Sort(int index, int count, Comparison<T> comp)
		{
			Debug.Assert(index + count <= _count);
			InternalList.Sort(_array, index, count, comp);
		}

		//IRange<T> IListSource<T>.Slice(int start, int count)
		//{
		//	//return new Slice_<T>(this, start, count);
		//	throw new NotImplementedException();
		//}
		//public Slice_<T> Slice(int start, int count)
		//{
		//	return new Slice_<T>(this, start, count);
		//}

		public InternalList<T> CopySection(int start, int subcount)
		{
			Debug.Assert((uint)start <= (uint)_count && subcount >= 0);
			if (subcount > _count - start)
				subcount = _count - start;

			var copy = new T[subcount];
			Array.Copy(_array, start, copy, 0, subcount);
			return new InternalList<T>(copy, subcount);
		}
	}
}
