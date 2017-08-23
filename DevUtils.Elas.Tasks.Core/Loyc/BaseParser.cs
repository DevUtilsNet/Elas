using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Local

namespace DevUtils.Elas.Tasks.Core.Loyc
{
	abstract class BaseParser<TT>
	{
		//private readonly List<Token<TT>> _tokensBuffer = new List<Token<TT>>();

		//private int _aheadCount;
		//private IEnumerable<Token<TT>> _tokenSource;
		//private IEnumerator<Token<TT>> _currentEnumerator;

		//protected IEnumerable<Token<TT>> TokenSource
		//{
		//	get { return _tokenSource; }
		//	set
		//	{
		//		_tokenSource = value;
		//		if (_tokenSource == null)
		//		{
		//			_currentEnumerator = null;
		//			return;
		//		}

		//		_currentEnumerator = _tokenSource.GetEnumerator();
		//		_tokensBuffer.Clear();

		//		MoveNext();
		//	}
		//}

		protected TT LA0
		{
			get
			{
				var ret = LT0.Type;
				return ret;
			}
		}

		protected TT LA(int k)
		{
			var token = LT(k);
			var ret = token.Type;
			return ret;
		}

		protected abstract Token<TT> LT0 { get; }

		protected abstract Token<TT> LT(int k);

		protected static HashSet<TT> NewSet(params TT[] items)
		{
			return new HashSet<TT>(items);
		}

		protected abstract void MoveNext();

		protected void Skip()
		{
			MoveNext();
		}

		protected virtual void Error(bool inverted, IEnumerable<TT> expected)
		{
			Error(String.Format("Error: '{0}': expected {1}", ToString(LT0.Type), ToString(inverted, expected)));
		}

		protected virtual string ToString(bool inverted, IEnumerable<TT> expected)
		{
			var expected2 = expected as TT[] ?? expected.ToArray();

			var num = expected2.Take(2).Count();
			if (num == 0)
			{
				return String.Format(inverted ? "anything" : "nothing");
			}
			if (inverted)
			{
				return String.Format("anything except {0}", ToString(false, expected2));
			}
			return num == 1 ? ToString(expected2.First()) : String.Join("|", expected2.Select(ToString));
		}

		protected abstract void Error(string message);
		protected abstract string ToString(TT tokenType);
		protected abstract bool Equals(TT left, TT right);

		protected Token<TT> Match(ICollection<TT> set, bool inverted = false)
		{
			var ret = LT0;
			if (set.Contains(ret.Type) == inverted)
			{
				Error(false, set);
			}
			else
			{
				MoveNext();
			}

			return ret;
		}

		protected Token<TT> Match(TT a)
		{
			var ret = LT0;
			if (!Equals(ret.Type, a))
			{
				Error(false, new[] { a });
			}
			else
			{
				MoveNext();
			}

			return ret;
		}

		protected Token<TT> Match(TT a, TT b)
		{
			var ret = LT0;
			if (!Equals(ret.Type, a) && !Equals(ret.Type, b))
			{
				Error(false, new[] { a, b });
			}
			else
			{
				MoveNext();
			}

			return ret;
		}

		protected Token<TT> Match(TT a, TT b, TT c)
		{
			var ret = LT0;
			if (!Equals(ret.Type, a) && !Equals(ret.Type, b) && !Equals(ret.Type, c))
			{
				Error(false, new[] { a, b, c });
			}
			else
			{
				MoveNext();
			}
			return ret;
		}

		protected Token<TT> Match(TT a, TT b, TT c, TT d)
		{
			var ret = LT0;
			if (!Equals(ret.Type, a) && !Equals(ret.Type, b) && !Equals(ret.Type, c) && !Equals(ret.Type, d))
			{
				Error(false, new[] { a, b, c, d });
			}
			else
			{
				MoveNext();
			}
			return ret;
		}

		protected Token<TT> MatchAny()
		{
			var ret = LT0;
			MoveNext();
			return ret;
		}

		protected Token<TT> MatchExcept()
		{
			var ret = LT0;
			MoveNext();
			return ret;
		}

		protected Token<TT> MatchExcept(ICollection<TT> set)
		{
			var ret = Match(set, true);
			return ret;
		}

		protected Token<TT> MatchExcept(TT a)
		{
			var ret = LT0;
			if (Equals(ret.Type, a))
			{
				Error(true, new[] { a });
			}
			else
			{
				MoveNext();
			}
			return ret;
		}

		protected Token<TT> MatchExcept(TT a, TT b)
		{
			var ret = LT0;
			if (Equals(ret.Type, a) || Equals(ret.Type, b))
			{
				Error(true, new[] { a, b });
			}
			else
			{
				MoveNext();
			}
			return ret;
		}

		protected Token<TT> MatchExcept(TT a, TT b, TT c)
		{
			var ret = LT0;
			if (Equals(ret.Type, a) || Equals(ret.Type, b) || Equals(ret.Type, c))
			{
				Error(true, new[] { a, b, c });
			}
			else
			{
				MoveNext();
			}
			return ret;
		}

		protected Token<TT> MatchExcept(TT a, TT b, TT c, TT d)
		{
			var ret = LT0;
			if (Equals(ret.Type, a) || Equals(ret.Type, b) || Equals(ret.Type, c) || Equals(ret.Type, d))
			{
				Error(true, new[] { a, b, c, d });
			}
			else
			{
				MoveNext();
			}

			return ret;
		}
	}
}