using System;
using System.Collections.Generic;

namespace DevUtils.Elas.Tasks.Core.Loyc.Collections
{
	/// <summary>A read-only list indexed by an integer.</summary>
	/// <remarks>
	/// The .NET collection classes have a very simple and coarse interface.
	/// <para/>
	/// Member list:
	/// <code>
	/// public T this[int index] { get; }
	/// public T TryGet(int index, ref bool fail);
	/// public Iterator&lt;T> GetIterator();
	/// public int Count { get; }
	/// public IEnumerator&lt;T> GetEnumerator();
	/// System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator();
	/// </code>
	/// The term "source" means a read-only collection, as opposed to a "sink" which
	/// is a write-only collection. The purpose of IListSource is to make it easier
	/// to implement a read-only list, by lifting IList's requirement to write 
	/// implementations for Add(), Remove(), etc. A secondary purpose is, of course,
	/// to guarantee users don't mistakenly call those methods on a read-only
	/// collection.
	/// <para/>
	/// I have often wanted to access the "next" or "previous" item in a list, e.g.
	/// during parsing, but it inconvenient if you have to worry about whether the 
	/// the current item is the first or last. In that case you must check whether
	/// the array index is valid, which is both inconvenient and wasteful, because
	/// the list class will check the array index again, and then the .NET runtime
	/// will check the index a third time when reading the internal array. To make
	/// this more efficient, IListSource has a TryGet() method that does not throw
	/// on failure, but returns default(T).
	/// <para/>
	/// As IListSource is supposed to be a simpler alternative to IList, I didn't
	/// want to require implementers to implement more than two indexers. There are
	/// two additional TryGet extension methods, though:
	/// <code>
	///     bool TryGet(int index, ref T value);
	///     T TryGet(int, T defaultValue);
	/// </code>
	/// If T is defined as "out" (covariant) in C# 4, these methods are not allowed 
	/// in IListSource anyway and MUST be extension methods.
	/// <para/>
	/// Note that "value" is a "ref" rather than an "out" parameter, unlike
	/// Microsoft's own TryGetValue() implementations. Using ref parameter allows
	/// the caller to choose his own default value in case TryGet() returns false.
	/// <para/>
	/// </remarks>
	public interface IListSource<out T>// : IReadOnlyList<T>
	{
		/// <summary>Gets the item at the specified index, and does not throw an
		/// exception on failure.</summary>
		/// <param name="index">An index in the range 0 to Count-1.</param>
		/// <param name="fail">A flag that is set on failure.</param>
		/// <returns>The element at the specified index, or default(T) if the index
		/// is not valid.</returns>
		/// <remarks>In my original design, the caller could provide a value to 
		/// return on failure, but this would not allow T to be marked as "out" in 
		/// C# 4. For the same reason, we cannot have a ref/out T parameter.
		/// Instead, the following extension methods are provided:
		/// <code>
		///     bool TryGet(int index, ref T value);
		///     T TryGet(int, T defaultValue);
		/// </code>
		/// </remarks>
		T TryGet(int index, out bool fail);

		///// <summary>Returns a sub-range of this list.</summary>
		///// <param name="start">The new range will start at this index in the current
		///// list (this location will be index [0] in the new range).</param>
		///// <param name="count">The desired number of elements in the new range,
		///// or int.MaxValue to get all elements until the end of the list.</param>
		///// <returns>Returns a sub-range of this range.</returns>
		///// <exception cref="ArgumentException">The start index was below zero.</exception>
		///// <remarks>The (start, count) range is allowed to be invalid, as long
		///// as start is zero or above. 
		///// <ul>
		///// <li>If count is below zero, or if start is above the original Count, the
		///// Count of the new slice is set to zero.</li>
		///// <li>if (start + count) is above the original Count, the Count of the new
		///// slice is reduced to <c>this.Count - start</c>. Implementation note:
		///// do not compute (start + count) because it may overflow. Instead, test
		///// whether (count > this.Count - start).</li>
		///// </ul>
		///// Most collections should use the following implementation:
		///// <pre>
		///// IRange&lt;T> IListSource&lt;T>.Slice(int start, int count) { return Slice(start, count); }
		///// public Slice_&lt;T> Slice(int start, int count) { return new Slice_&lt;T>(this, start, count); }
		///// </pre>
		///// </remarks>
		//IRange<T> Slice(int start, int count = int.MaxValue);
		/*
			IRange<T> IListSource<T>.Slice(int start, int count) { return Slice(start, count); }
			public Slice_<T> Slice(int start, int count) { return new Slice_<T>(this, start, count); }
		*/
	}
}
