using System;

namespace DevUtils.Elas.Tasks.Core.Loyc
{
	/// <summary> A token. </summary>
	///
	/// <typeparam name="T"> Generic type parameter. </typeparam>
	struct Token<T> : ICloneable, IEquatable<Token<T>>
	{
		/// <summary>Token type.</summary>
		public T Type { get; private set; }

		/// <summary>Returns StartIndex + Length.</summary>
		public int EndIndex;

		/// <summary>Location in the original source file where the token starts, or
		/// -1 for a synthetic token.</summary>
		public int StartIndex;

		/// <summary>Length of the token in the source file, or 0 for a synthetic 
		/// or implied token.</summary>
		public int Length { get { return EndIndex - StartIndex; } }

		public Token(T type, int startIndex, int endIndex) : this()
		{
			Type = type;
			StartIndex = startIndex;
			EndIndex = endIndex;
		}

		public Token<T> ChangeType(T type)
		{
			var ret = this;
			ret.Type = type;
			return ret;
		}

		#region ToString, Equals, GetHashCode

		public override string ToString()
		{
			var ret = string.Format("Type: {0} Length: {1}", Type, Length);
			return ret;
		}

		public override bool Equals(object obj)
		{
			return obj is Token<T> && Equals((Token<T>)obj);
		}

		public bool Equals(Token<T> other)
		{
			return Equals(Type, other.Type);
		}
		public override int GetHashCode()
		{
			return Type.GetHashCode();
		}

		#endregion

		object ICloneable.Clone()
		{
			return this;
		}
	}
}
