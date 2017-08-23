using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevUtils.Elas.Tasks.Core.Collections.Extensions;

namespace DevUtils.Elas.Tasks.Core.Loyc
{
	abstract class BaseLexer
	{
		const int CachedBlockSize = 0x100;

		private readonly char[] _cache = new char[CachedBlockSize];

		private int _cacheStart;
		private int _cacheLength;
		private int _inputPosition;
		private ICharSource _charSource;

		protected int LA0 { get; private set; }

		public ICharSource CharSource
		{
			get { return _charSource; }
			set
			{
				LineNumber = 1;
				_lineStartAt = 0;
				_charSource = value;
				InputPosition = 0;
			}
		}

		public int LineNumber { get; protected set; }

		public int LinePosition
		{
			get
			{
				var ret = InputPosition - _lineStartAt;
				return ret;
			}
		}

		public int InputPosition
		{
			get { return _inputPosition; }
			set
			{
				_inputPosition = value;
				LA0 = LA(0);
			}
		}

		protected static HashSet<int> NewSet(params int[] items) { return new HashSet<int>(items); }

		protected static HashSet<int> NewSetOfRanges(params int[] ranges)
		{
			var set = new HashSet<int>();
			for (var r = 0; r < ranges.Length; r += 2)
			{
				for (var i = ranges[r]; i <= ranges[r + 1]; i++)
				{
					set.Add(i);
				}
			}
			return set;
		}

		protected BaseLexer()
		{
			
		}

		protected BaseLexer(ICharSource source)
		{
			CharSource = source;
		}

		protected abstract void Error(int lookaheadIndex, string message);

		protected int LA(int i)
		{
			var offset = _inputPosition + i;
			if (offset < _cacheStart || offset >= _cacheStart + _cacheLength)
			{
				_cacheStart = offset;
				_cacheLength = CharSource.Read(_cache, _cacheStart, 0, _cache.Length);
			}

			if (_cacheLength == 0 || offset - _cacheStart >= _cache.Length)
			{
				return -1;
			}

			var ret = _cache[offset - _cacheStart];
			return ret;
		}

		private void MoveNext()
		{
			if (LA0 != -1)
			{
				InputPosition++;
			}
		}

		/// <summary>Increments InputPosition. Called by LLLPG when prediction 
		/// already verified the input (and caller doesn't save LA(0))</summary>
		protected void Skip()
		{
			MoveNext();
		}

		protected int _lineStartAt;

		/// <summary>The lexer should call this method, which updates _lineStartAt 
		/// and LineNumber, each time it encounters a newline, even inside 
		/// comments and strings.</summary>
		protected virtual void AfterNewline()
		{
			if (_lineStartAt != InputPosition)
			{
				LineNumber++;
				_lineStartAt = InputPosition;
			}
		}

		/// <summary>Default newline parser that matches '\n' or '\r' unconditionally.</summary>
		/// <remarks>
		/// You can use this implementation in an LLLPG lexer with "extern", like so:
		/// <c>extern rule Newline @[ '\r' + '\n'? | '\n' ];</c>
		/// By using this implementation everywhere in the grammar in which a 
		/// newline is allowed (even inside comments and strings), you can ensure
		/// that <see cref="AfterNewline()"/> is called, so that the line number
		/// is updated properly.
		/// </remarks>
		protected void Newline()
		{
			var la0 = LA0;
			if (la0 == '\r')
			{
				Skip();
				for (; ; )
				{
					la0 = LA0;
					if (la0 == '\r')
					{
						Skip();
					}
					else
					{
						break;
					}
				}
				la0 = LA0;
				if (la0 == '\n')
				{
					Skip();
				}
			}
			else
			{
				Match('\n');
			}
			AfterNewline();
		}

		#region Normal matching

		protected int MatchAny()
		{
			var la = LA0;
			MoveNext();
			return la;
		}
		protected int Match(HashSet<int> set)
		{
			var la = LA0;
			if (!set.Contains(la))
			{
				Error(false, set);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int Match(int a)
		{
			var la = LA0;
			if (la != a)
			{
				Error(false, a, a);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int Match(int a, int b)
		{
			var la = LA0;
			if (la != a && la != b)
			{
				Error(false, a, a, b, b);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int Match(int a, int b, int c)
		{
			var la = LA0;
			if (la != a && la != b && la != c)
			{
				Error(false, a, a, b, b, c, c);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int Match(int a, int b, int c, int d)
		{
			var la = LA0;
			if (la != a && la != b && la != c && la != d)
			{
				Error(false, a, a, b, b, c, c, d, d);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchRange(int aLo, int aHi)
		{
			var la = LA0;
			if ((la < aLo || la > aHi))
			{
				Error(false, aLo, aHi);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchRange(int aLo, int aHi, int bLo, int bHi)
		{
			var la = LA0;
			if ((la < aLo || la > aHi) && (la < bLo || la > bHi))
			{
				Error(false, aLo, aHi, bLo, bHi);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExcept()
		{
			var la = LA0;
			if (la == -1)
			{
				Error(true, -1, -1);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExcept(HashSet<int> set)
		{
			var la = LA0;
			if (set.Contains(la))
			{
				Error(true, set);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExcept(int a)
		{
			var la = LA0;
			if (la == -1 || la == a)
			{
				Error(true, a, a);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExcept(int a, int b)
		{
			var la = LA0;
			if (la == -1 || la == a || la == b)
			{
				Error(true, a, a, b, b);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExcept(int a, int b, int c)
		{
			var la = LA0;
			if (la == -1 || la == a || la == b || la == c)
			{
				Error(true, a, a, b, b, c, c);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExceptRange(int aLo, int aHi)
		{
			var la = LA0;
			if (la == -1 || (la >= aLo && la <= aHi))
			{
				Error(true, aLo, aHi);
			}
			else
			{
				MoveNext();
			}
			return la;
		}
		protected int MatchExceptRange(int aLo, int aHi, int bLo, int bHi)
		{
			var la = LA0;
			if (la == -1 || (la >= aLo && la <= aHi) || (la >= bLo && la <= bHi))
			{
				Error(true, aLo, aHi, bLo, bHi);
			}
			else
			{
				MoveNext();
			}
			return la;
		}

		#endregion

		#region Try-matching

		///// <summary>A helper class used by LLLPG for backtracking.</summary>
		//protected struct SavePosition : IDisposable
		//{
		//	int _oldPosition;
		//	readonly BaseLexer _lexer;

		//	public SavePosition(BaseLexer lexer, int lookaheadAmt)
		//	{ _lexer = lexer; _oldPosition = lexer.InputPosition; lexer.InputPosition += lookaheadAmt; }
		//	public void Dispose() { _lexer.InputPosition = _oldPosition; }
		//}

		protected bool TryMatch(HashSet<int> set)
		{
			if (!set.Contains(LA0))
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatch(int a)
		{
			if (LA0 != a)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatch(int a, int b)
		{
			var la = LA0;
			if (la != a && la != b)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatch(int a, int b, int c)
		{
			var la = LA0;
			if (la != a && la != b && la != c)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchRange(int aLo, int aHi)
		{
			var la = LA0;
			if (la < aLo || la > aHi)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchRange(int aLo, int aHi, int bLo, int bHi)
		{
			var la = LA0;
			if ((la < aLo || la > aHi) && (la < bLo || la > bHi))
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExcept()
		{
			if (LA0 == -1)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExcept(HashSet<int> set)
		{
			if (set.Contains(LA0))
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExcept(int a)
		{
			var la = LA0;
			if (la == -1 || la == a)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExcept(int a, int b)
		{
			var la = LA0;
			if (la == -1 || la == a || la == b)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExcept(int a, int b, int c)
		{
			var la = LA0;
			if (la == -1 || la == a || la == b || la == c)
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExceptRange(int aLo, int aHi)
		{
			var la = LA0;
			if (la == -1 || (la >= aLo && la <= aHi))
			{
				return false;
			}
			MoveNext();
			return true;
		}
		protected bool TryMatchExceptRange(int aLo, int aHi, int bLo, int bHi)
		{
			var la = LA0;
			if (la == -1 || (la >= aLo && la <= aHi) || (la >= bLo && la <= bHi))
			{
				return false;
			}
			MoveNext();
			return true;
		}

		#endregion

		#region Error

		protected virtual void Error(bool inverted, params int[] ranges)
		{
			Error(inverted, (IList<int>) ranges);
		}

		protected virtual void Error(bool inverted, IList<int> ranges)
		{
			var rangesDescr = RangesToString(ranges);
			var input = new StringBuilder();
			PrintChar(LA0, input);
			if (inverted)
			{
				Error(0, String.Format("{0}: expected a character other than {1}", input, rangesDescr));
			}
			else if (ranges.Count > 2)
			{
				Error(0, String.Format("{0}: expected one of {1}", input, rangesDescr));
			}
			else
			{
				Error(0, String.Format("{0}: expected {1}", input, rangesDescr));
			}
		}

		protected virtual void Error(bool inverted, HashSet<int> set)
		{
			var array = set.ToArray();
			array.Sort();
			var list = new List<int>();
			for (var i = 0; i < array.Length; i++)
			{
				int j;
				for (j = i + 1; j < array.Length && array[j] == array[i] + 1; j++)
				{
				}
				list.Add(i);
				list.Add(j - 1);
			}
			Error(inverted, list);
		}

		private static string RangesToString(IList<int> ranges)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < ranges.Count; i += 2)
			{
				if (i != 0)
					sb.Append(' ');
				int lo = ranges[i], hi = ranges[i + 1];
				PrintChar(lo, sb);
				if (hi > lo)
				{
					sb.Append(hi > lo + 1 ? '-' : ' ');
					PrintChar(hi, sb);
				}
			}
			return sb.ToString();
		}

		private static void PrintChar(int c, StringBuilder sb)
		{
			if (c == -1)
				sb.Append("EOF");
			else if (c >= 0 && c < 0xFFFC)
			{
				sb.Append('\'');
				EscapeCStyle((char) c, sb);
				sb.Append('\'');
			}
			else
				sb.Append(c);
		}

		private static void EscapeCStyle(char c, StringBuilder @out, EscapeC flags = EscapeC.Default, char quoteType = '\0')
		{
			if (c > 255 && (flags & (EscapeC.Unicode | EscapeC.NonAscii)) != 0)
			{
				@out.AppendFormat(@"\u{0:x0000}", (int) c);
			}
			else if (c == '\"' && (flags & EscapeC.DoubleQuotes) != 0)
			{
				@out.Append("\\\"");
			}
			else if (c == '\'' && (flags & EscapeC.SingleQuotes) != 0)
			{
				@out.Append("\\'");
			}
			else if (c == quoteType)
			{
				@out.Append('\\');
				@out.Append(c);
			}
			else if (c < 32)
			{
				switch (c)
				{
					case '\n':
						@out.Append(@"\n");
						break;
					case '\r':
						@out.Append(@"\r");
						break;
					case '\0':
						@out.Append(@"\0");
						break;
					default:
						if ((flags & EscapeC.ABFV) != 0)
						{
							if (c == '\a')
							{
								// 7 (alert)
								@out.Append(@"\a");
								return;
							}
							if (c == '\b')
							{
								// 8 (backspace)
								@out.Append(@"\b");
								return;
							}
							if (c == '\f')
							{
								// 12 (form feed)
								@out.Append(@"\f");
								return;
							}
							if (c == '\v')
							{
								// 11 (vertical tab)
								@out.Append(@"\v");
								return;
							}
						}
						if ((flags & EscapeC.Control) != 0)
						{
							if (c == '\t')
								@out.Append(@"\t");
							else
								@out.AppendFormat(@"\x{0:X2}", (int) c);
						}
						else
							@out.Append(c);
						break;
				}
			}
			else if (c == '\\')
			{
				@out.Append(@"\\");
			}
			else if (c > 127 && (flags & EscapeC.NonAscii) != 0)
			{
				@out.AppendFormat(@"\x{0:X2}", (int) c);
			}
			else
			{
				@out.Append(c);
			}
		}

		#endregion

		protected virtual void Check(bool expectation, string expectedDescr = "")
		{
			if (!expectation)
			{
				Error(0, String.Format("An expected condition was false: {0}", expectedDescr));
			}
		}
	}
}
