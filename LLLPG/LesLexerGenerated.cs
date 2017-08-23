using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loyc;
using Loyc.LLParserGenerator;
using Loyc.Syntax;
using Loyc.Syntax.Lexing;

namespace Loyc.Syntax.Les
{
	using TT = TokenType;

	public partial class LesLexer
	{
		public void Newline()
		{
			int la0;
			// Line 0: ([\r] ([\n])? | [\n])
			la0 = LA0;
			if (la0 == '\r') {
				Skip();
				// Line 0: ([\n])?
				la0 = LA0;
				if (la0 == '\n')
					Skip();
			} else
				Match('\n');
			_lineStartAt = InputPosition;
			_lineNumber++;
			_value = WhitespaceTag.Value;
		}
		private void DotIndent()
		{
			int la0, la1;
			_type = TT.Spaces;
			Skip();
			Skip();
			// Line 0: ([\t ])*
			for (;;) {
				la0 = LA0;
				if (la0 == '\t' || la0 == ' ')
					Skip();
				else
					break;
			}
			// Line 0: ([.] [\t ] ([\t ])*)*
			for (;;) {
				la0 = LA0;
				if (la0 == '.') {
					la1 = LA(1);
					if (la1 == '\t' || la1 == ' ') {
						Skip();
						Skip();
						// Line 0: ([\t ])*
						for (;;) {
							la0 = LA0;
							if (la0 == '\t' || la0 == ' ')
								Skip();
							else
								break;
						}
					} else
						break;
				} else
					break;
			}
			_indentLevel = MeasureIndent(_indent = Source.Substring(_startPosition, InputPosition - _startPosition));
			_value = WhitespaceTag.Value;
		}
		public void Spaces()
		{
			int la0;
			Match('\t', ' ');
			// Line 0: ([\t ])*
			for (;;) {
				la0 = LA0;
				if (la0 == '\t' || la0 == ' ')
					Skip();
				else
					break;
			}
			if (_lineStartAt == _startPosition) _indentLevel = MeasureIndent(_indent = Source.Substring(_startPosition, InputPosition - _startPosition));
			_value = WhitespaceTag.Value;
		}
		public void SLComment()
		{
			int la0;
			Match('/');
			Match('/');
			// Line 0: ([^\$\n\r])*
			for (;;) {
				la0 = LA0;
				if (!(la0 == -1 || la0 == '\n' || la0 == '\r'))
					Skip();
				else
					break;
			}
			_value = WhitespaceTag.Value;
		}
		public void MLComment()
		{
			int la0, la1;
			Match('/');
			Match('*');
			// Line 0: nongreedy(MLComment / [^\$])*
			for (;;) {
				la0 = LA0;
				if (la0 == '*') {
					la1 = LA(1);
					if (la1 == -1 || la1 == '/')
						break;
					else
						Skip();
				} else if (la0 == -1)
					break;
				else if (la0 == '/') {
					la1 = LA(1);
					if (la1 == '*')
						MLComment();
					else
						Skip();
				} else
					Skip();
			}
			Match('*');
			Match('/');
			_value = WhitespaceTag.Value;
		}
		static readonly HashSet<int> SQString_set0 = NewSet(-1, 9, 10, 13, ' ', '\'');
		public void SQString()
		{
			int la0, la1;
			_parseNeeded = false;
			Match('\'');
			// Line 0: ( [\\] [^\$\n\r] / [^\$\n\r'\\] /  )
			la0 = LA0;
			if (la0 == '\\') {
				la1 = LA(1);
				if (!(la1 == -1 || la1 == '\n' || la1 == '\r')) {
					Skip();
					Skip();
					_parseNeeded = true;
				} else
					_parseNeeded = true
			} else if (!(la0 == -1 || la0 == '\n' || la0 == '\r' || la0 == '\''))
				Skip();
			else
				_parseNeeded = true
			// Line 0: ([^\$\t\n\r '])*
			for (;;) {
				la0 = LA0;
				if (!SQString_set0.Contains(la0)) {
					Skip();
					_parseNeeded = true;
				} else
					break;
			}
			// Line 0: (['] / )
			la0 = LA0;
			if (la0 == '\'')
				Skip();
			ParseSQStringValue();
		}
		public void DQString()
		{
			int la0, la1;
			_parseNeeded = false;
			// Line 0: (["] ([\\] [^\$] | [^\$\n\r"\\])* ["] | [#] ["] (["] ["] / [^\$"])* ["])
			la0 = LA0;
			if (la0 == '"') {
				Skip();
				// Line 0: ([\\] [^\$] | [^\$\n\r"\\])*
				for (;;) {
					la0 = LA0;
					if (la0 == '\\') {
						Skip();
						MatchExcept();
						_parseNeeded = true;
					} else if (!(la0 == -1 || la0 == '\n' || la0 == '\r' || la0 == '"'))
						Skip();
					else
						break;
				}
				Match('"');
			} else {
				_style = NodeStyle.Alternate;
				Match('#');
				Match('"');
				// Line 0: (["] ["] / [^\$"])*
				for (;;) {
					la0 = LA0;
					if (la0 == '"') {
						la1 = LA(1);
						if (la1 == '"') {
							Skip();
							Skip();
							_parseNeeded = true;
						} else
							break;
					} else if (la0 != -1)
						Skip();
					else
						break;
				}
				Match('"');
			}
			ParseStringValue(false);
		}
		public void TQString()
		{
			int la0, la1, la2;
			_parseNeeded = true;
			// Line 0: (["] ["] ["] nongreedy([\\] [\\] ["] / [^\$])* ["] ["] ["] | ['] ['] ['] nongreedy([\\] [\\] ['] / [^\$])* ['] ['] ['])
			la0 = LA0;
			if (la0 == '"') {
				_style = NodeStyle.Alternate;
				Skip();
				Match('"');
				Match('"');
				// Line 0: nongreedy([\\] [\\] ["] / [^\$])*
				for (;;) {
					la0 = LA0;
					if (la0 == '"') {
						la1 = LA(1);
						if (la1 == '"') {
							la2 = LA(2);
							if (la2 == -1 || la2 == '"')
								break;
							else
								Skip();
						} else if (la1 == -1)
							break;
						else
							Skip();
					} else if (la0 == -1)
						break;
					else if (la0 == '\\') {
						la1 = LA(1);
						if (la1 == '\\') {
							la2 = LA(2);
							if (la2 == '"') {
								Skip();
								Skip();
								Skip();
							} else
								Skip();
						} else
							Skip();
					} else
						Skip();
				}
				Match('"');
				Match('"');
				Match('"');
			} else {
				_style = NodeStyle.Alternate | NodeStyle.Alternate2;
				Match('\'');
				Match('\'');
				Match('\'');
				// Line 0: nongreedy([\\] [\\] ['] / [^\$])*
				for (;;) {
					la0 = LA0;
					if (la0 == '\'') {
						la1 = LA(1);
						if (la1 == '\'') {
							la2 = LA(2);
							if (la2 == -1 || la2 == '\'')
								break;
							else
								Skip();
						} else if (la1 == -1)
							break;
						else
							Skip();
					} else if (la0 == -1)
						break;
					else if (la0 == '\\') {
						la1 = LA(1);
						if (la1 == '\\') {
							la2 = LA(2);
							if (la2 == '\'') {
								Skip();
								Skip();
								Skip();
							} else
								Skip();
						} else
							Skip();
					} else
						Skip();
				}
				Match('\'');
				Match('\'');
				Match('\'');
			}
			ParseStringValue(true);
		}
		public void BQString()
		{
			BQString2();
			ParseBQStringValue();
		}
		private void BQString2()
		{
			int la0;
			_parseNeeded = false;
			Match('`');
			// Line 0: ([\\] [^\$] | [^\$\n\r\\`])*
			for (;;) {
				la0 = LA0;
				if (la0 == '\\') {
					Skip();
					MatchExcept();
					_parseNeeded = true;
				} else if (!(la0 == -1 || la0 == '\n' || la0 == '\r' || la0 == '`'))
					Skip();
				else
					break;
			}
			Match('`');
		}
		private void IdExtLetter()
		{
			Check(char.IsLetter((char) LA0), "char.IsLetter((char) LA0)");
			MatchRange('', '￼');
		}
		static readonly HashSet<int> NormalId_set0 = NewSetOfRanges('#', '#', 'A', 'Z', '_', '_', 'a', 'z');
		static readonly HashSet<int> NormalId_set1 = NewSetOfRanges('#', '#', '\'', '\'', '0', '9', 'A', 'Z', '_', '_', 'a', 'z');
		public void NormalId()
		{
			int la0;
			// Line 0: ([#A-Z_a-z] | IdExtLetter)
			la0 = LA0;
			if (NormalId_set0.Contains(la0))
				Skip();
			else
				IdExtLetter();
			// Line 0: ([#'0-9A-Z_a-z] | IdExtLetter)*
			for (;;) {
				la0 = LA0;
				if (NormalId_set1.Contains(la0))
					Skip();
				else if (la0 >= '' && la0 <= '￼') {
					if (char.IsLetter((char) LA0))
						IdExtLetter();
					else
						break;
				} else
					break;
			}
		}
		private void CommentStart()
		{
			Match('/');
			Match('*', '/');
		}
		private bool Try_Scan_CommentStart(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_CommentStart();
		}
		private bool Scan_CommentStart()
		{
			if (!TryMatch('/'))
				return false;
			if (!TryMatch('*', '/'))
				return false;
			return true;
		}
		static readonly HashSet<int> FancyId_set0 = NewSetOfRanges('!', '!', '#', '\'', '*', '+', '-', ':', '<', '?', 'A', 'Z', '\\', '\\', '^', '_', 'a', 'z', '|', '|', '~', '~');
		public void FancyId()
		{
			int la0;
			// Line 0: (BQString2 | (&!(CommentStart) [!#-'*+\--:<-?A-Z\\^_a-z|~] | IdExtLetter) (&!(CommentStart) [!#-'*+\--:<-?A-Z\\^_a-z|~] | IdExtLetter)*)
			la0 = LA0;
			if (la0 == '`')
				BQString2();
			else {
				// Line 0: (&!(CommentStart) [!#-'*+\--:<-?A-Z\\^_a-z|~] | IdExtLetter)
				la0 = LA0;
				if (FancyId_set0.Contains(la0)) {
					Check(!Try_Scan_CommentStart(0), "!(CommentStart)");
					Skip();
				} else
					IdExtLetter();
				// Line 0: (&!(CommentStart) [!#-'*+\--:<-?A-Z\\^_a-z|~] | IdExtLetter)*
				for (;;) {
					la0 = LA0;
					if (FancyId_set0.Contains(la0)) {
						if (!Try_Scan_CommentStart(0))
							Skip();
						else
							break;
					} else if (la0 >= '' && la0 <= '￼') {
						if (char.IsLetter((char) LA0))
							IdExtLetter();
						else
							break;
					} else
						break;
				}
			}
		}
		public void Symbol()
		{
			_parseNeeded = false;
			Match('@');
			Match('@');
			FancyId();
			ParseSymbolValue();
		}
		static readonly HashSet<int> Id_set0 = NewSetOfRanges('#', '#', 'A', 'Z', '_', '_', 'a', 'z', 128, 65532);
		private void Id()
		{
			int la0;
			_parseNeeded = false;
			// Line 0: (NormalId | [@] FancyId)
			la0 = LA0;
			if (Id_set0.Contains(la0))
				NormalId();
			else {
				Match('@');
				FancyId();
				_parseNeeded = true;
			}
			ParseIdValue();
		}
		private void Comma()
		{
			Skip();
			_type = TT.Comma;
			_value = _Comma;
		}
		private void Semicolon()
		{
			Skip();
			_type = TT.Semicolon;
			_value = _Semicolon;
		}
		private void At()
		{
			Skip();
			_type = TT.At; _value = GSymbol.Empty;
		}
		private void Operator()
		{
			Check(!Try_Scan_CommentStart(0), "!(CommentStart)");
			Skip();
			// Line 0: (&!(CommentStart) [!$-&*+\--/:<-?^|~])*
			for (;;) {
				switch (LA0) {
				case '!':
				case '$':
				case '%':
				case '&':
				case '*':
				case '+':
				case '-':
				case '.':
				case '/':
				case ':':
				case '<':
				case '=':
				case '>':
				case '?':
				case '^':
				case '|':
				case '~':
					{
						if (!Try_Scan_CommentStart(0))
							Skip();
						else
							goto stop;
					}
					break;
				default:
					goto stop;
				}
			}
		stop:;
			ParseNormalOp();
		}
		private void BackslashOp()
		{
			int la0, la1;
			Skip();
			// Line 0: (FancyId)?
			la0 = LA0;
			if (la0 == '`') {
				la1 = LA(1);
				if (!(la1 == -1 || la1 == '\n' || la1 == '\r'))
					FancyId();
			} else if (FancyId_set0.Contains(la0)) {
				if (!Try_Scan_CommentStart(0))
					FancyId();
			} else if (la0 >= '' && la0 <= '￼') {
				if (char.IsLetter((char) LA0))
					FancyId();
			}
			ParseBackslashOp();
		}
		public void LParen()
		{
			Match('(');
		}
		public void RParen()
		{
			Match(')');
		}
		public void LBrack()
		{
			Match('[');
		}
		public void RBrack()
		{
			Match(']');
		}
		public void LBrace()
		{
			Match('{');
		}
		public void RBrace()
		{
			Match('}');
		}
		private void DecDigits()
		{
			int la0, la1;
			MatchRange('0', '9');
			// Line 0: ([0-9])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '9')
					Skip();
				else
					break;
			}
			// Line 0: ([_] [0-9] ([0-9])*)*
			for (;;) {
				la0 = LA0;
				if (la0 == '_') {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '9') {
						Skip();
						Skip();
						// Line 0: ([0-9])*
						for (;;) {
							la0 = LA0;
							if (la0 >= '0' && la0 <= '9')
								Skip();
							else
								break;
						}
					} else
						break;
				} else
					break;
			}
		}
		static readonly HashSet<int> HexDigits_set0 = NewSetOfRanges('0', '9', 'A', 'F', 'a', 'f');
		private void HexDigits()
		{
			int la0, la1;
			Skip();
			// Line 0: greedy([0-9A-Fa-f])*
			for (;;) {
				la0 = LA0;
				if (HexDigits_set0.Contains(la0))
					Skip();
				else
					break;
			}
			// Line 0: ([_] [0-9A-Fa-f] greedy([0-9A-Fa-f])*)*
			for (;;) {
				la0 = LA0;
				if (la0 == '_') {
					la1 = LA(1);
					if (HexDigits_set0.Contains(la1)) {
						Skip();
						Skip();
						// Line 0: greedy([0-9A-Fa-f])*
						for (;;) {
							la0 = LA0;
							if (HexDigits_set0.Contains(la0))
								Skip();
							else
								break;
						}
					} else
						break;
				} else
					break;
			}
		}
		private void BinDigits()
		{
			int la0, la1;
			Skip();
			// Line 0: ([01])*
			for (;;) {
				la0 = LA0;
				if (la0 >= '0' && la0 <= '1')
					Skip();
				else
					break;
			}
			// Line 0: ([_] [01] ([01])*)*
			for (;;) {
				la0 = LA0;
				if (la0 == '_') {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '1') {
						Skip();
						Skip();
						// Line 0: ([01])*
						for (;;) {
							la0 = LA0;
							if (la0 >= '0' && la0 <= '1')
								Skip();
							else
								break;
						}
					} else
						break;
				} else
					break;
			}
		}
		private void DecNumber()
		{
			int la0, la1;
			_numberBase = 10;
			// Line 0: ([.] DecDigits | DecDigits ([.] DecDigits)?)
			la0 = LA0;
			if (la0 == '.') {
				_isFloat = true;
				Skip();
				DecDigits();
			} else {
				DecDigits();
				// Line 0: ([.] DecDigits)?
				la0 = LA0;
				if (la0 == '.') {
					la1 = LA(1);
					if (la1 >= '0' && la1 <= '9') {
						_isFloat = true;
						Skip();
						DecDigits();
					}
				}
			}
			// Line 0: ([Ee] ([+\-])? DecDigits)?
			la0 = LA0;
			if (la0 == 'E' || la0 == 'e') {
				la1 = LA(1);
				if (la1 == '+' || la1 == '-' || la1 >= '0' && la1 <= '9') {
					_isFloat = true;
					Skip();
					// Line 0: ([+\-])?
					la0 = LA0;
					if (la0 == '+' || la0 == '-')
						Skip();
					DecDigits();
				}
			}
		}
		private void HexNumber()
		{
			int la0, la1;
			_numberBase = 16; _style = NodeStyle.Alternate;
			Skip();
			Skip();
			// Line 0: (HexDigits)?
			la0 = LA0;
			if (HexDigits_set0.Contains(la0))
				HexDigits();
			// Line 0: ([.] HexDigits)?
			la0 = LA0;
			if (la0 == '.') {
				la1 = LA(1);
				if (HexDigits_set0.Contains(la1)) {
					_isFloat = true;
					Skip();
					HexDigits();
				}
			}
			// Line 0: ([Pp] ([+\-])? DecDigits)?
			la0 = LA0;
			if (la0 == 'P' || la0 == 'p') {
				la1 = LA(1);
				if (la1 == '+' || la1 == '-' || la1 >= '0' && la1 <= '9') {
					_isFloat = true;
					Skip();
					// Line 0: ([+\-])?
					la0 = LA0;
					if (la0 == '+' || la0 == '-')
						Skip();
					DecDigits();
				}
			}
		}
		private void BinNumber()
		{
			int la0, la1;
			_numberBase = 2; _style = NodeStyle.Alternate2;
			Skip();
			Skip();
			// Line 0: (BinDigits)?
			la0 = LA0;
			if (la0 >= '0' && la0 <= '1')
				BinDigits();
			// Line 0: ([.] BinDigits)?
			la0 = LA0;
			if (la0 == '.') {
				la1 = LA(1);
				if (la1 >= '0' && la1 <= '1') {
					_isFloat = true;
					Skip();
					BinDigits();
				}
			}
			// Line 0: ([Pp] ([+\-])? DecDigits)?
			la0 = LA0;
			if (la0 == 'P' || la0 == 'p') {
				la1 = LA(1);
				if (la1 == '+' || la1 == '-' || la1 >= '0' && la1 <= '9') {
					_isFloat = true;
					Skip();
					// Line 0: ([+\-])?
					la0 = LA0;
					if (la0 == '+' || la0 == '-')
						Skip();
					DecDigits();
				}
			}
		}
		public void Number()
		{
			int la0;
			_isFloat = false;
			_isNegative = false;
			// Line 0: ([\-])?
			la0 = LA0;
			if (la0 == '-') {
				Skip();
				_isNegative = true;
			}
			_typeSuffix = GSymbol.Get("");
			// Line 0: ( HexNumber / BinNumber / DecNumber )
			la0 = LA0;
			if (la0 == '0') {
				switch (LA(1)) {
				case 'X':
				case 'x':
					HexNumber();
					break;
				case 'B':
				case 'b':
					BinNumber();
					break;
				default:
					DecNumber();
					break;
				}
			} else
				DecNumber();
			// Line 0: ( [Ff] | [Dd] | [Mm] | [Ll] ([Uu])? | [Uu] ([Ll])? )?
			switch (LA0) {
			case 'F':
			case 'f':
				{
					Skip();
					_typeSuffix=_F; _isFloat=true;
				}
				break;
			case 'D':
			case 'd':
				{
					Skip();
					_typeSuffix=_D; _isFloat=true;
				}
				break;
			case 'M':
			case 'm':
				{
					Skip();
					_typeSuffix=_M; _isFloat=true;
				}
				break;
			case 'L':
			case 'l':
				{
					Skip();
					_typeSuffix = _L;
					// Line 0: ([Uu])?
					la0 = LA0;
					if (la0 == 'U' || la0 == 'u') {
						Skip();
						_typeSuffix = _UL;
					}
				}
				break;
			case 'U':
			case 'u':
				{
					Skip();
					_typeSuffix = _U;
					// Line 0: ([Ll])?
					la0 = LA0;
					if (la0 == 'L' || la0 == 'l') {
						Skip();
						_typeSuffix = _UL;
					}
				}
				break;
			}
			ParseNumberValue();
		}
		static readonly HashSet<int> Token_set0 = NewSetOfRanges('A', 'Z', '_', '_', 'a', 'z', 128, 65532);
		public void Token()
		{
			int la0, la1, la2;
			// Line 0: ( &{InputPosition == 0} Shebang / Symbol / default Id / Spaces / Newline / DotIndent / SLComment / MLComment / Number / TQString / DQString / SQString / BQString / Comma / Semicolon / LParen / LBrack / LBrace / RParen / RBrack / RBrace / At / BackslashOp / Operator )
			do {
				la0 = LA0;
				switch (la0) {
				case '#':
					{
						if (InputPosition == 0) {
							la1 = LA(1);
							if (la1 == '!') {
								_type = TT.Shebang;
								Shebang();
							} else
								goto match3;
						} else
							goto match3;
					}
					break;
				case '@':
					{
						la1 = LA(1);
						if (la1 == '@') {
							la2 = LA(2);
							if (la2 == '`')
								goto match2;
							else if (FancyId_set0.Contains(la2)) {
								if (!Try_Scan_CommentStart(2))
									goto match2;
								else
									goto match22;
							} else if (la2 >= '' && la2 <= '￼') {
								if (char.IsLetter((char) LA0))
									goto match2;
								else
									goto match22;
							} else
								goto match22;
						} else if (la1 == '`') {
							la2 = LA(2);
							if (!(la2 == -1 || la2 == '\n' || la2 == '\r'))
								goto match3;
							else
								goto match22;
						} else if (FancyId_set0.Contains(la1)) {
							if (!Try_Scan_CommentStart(1))
								goto match3;
							else
								goto match22;
						} else if (la1 >= '' && la1 <= '￼') {
							if (char.IsLetter((char) LA0))
								goto match3;
							else
								goto match22;
						} else
							goto match22;
					}
				case '\t':
				case ' ':
					{
						_type = TT.Spaces;
						Spaces();
					}
					break;
				case '\n':
				case '\r':
					{
						_type = TT.Newline;
						Newline();
					}
					break;
				case '.':
					{
						if (_startPosition == _lineStartAt) {
							if (!Try_Scan_CommentStart(0)) {
								la1 = LA(1);
								if (la1 == '\t' || la1 == ' ')
									DotIndent();
								else if (la1 >= '0' && la1 <= '9')
									goto match9;
								else
									Operator();
							} else {
								la1 = LA(1);
								if (la1 == '\t' || la1 == ' ')
									DotIndent();
								else if (la1 >= '0' && la1 <= '9')
									goto match9;
								else
									goto match3;
							}
						} else if (!Try_Scan_CommentStart(0)) {
							la1 = LA(1);
							if (la1 >= '0' && la1 <= '9')
								goto match9;
							else
								Operator();
						} else
							goto match9;
					}
					break;
				case '/':
					{
						if (!Try_Scan_CommentStart(0)) {
							la1 = LA(1);
							if (la1 == '/')
								goto match7;
							else if (la1 == '*') {
								la2 = LA(2);
								if (la2 != -1)
									goto match8;
								else
									Operator();
							} else
								Operator();
						} else {
							la1 = LA(1);
							if (la1 == '/')
								goto match7;
							else if (la1 == '*')
								goto match8;
							else
								goto match3;
						}
					}
					break;
				case '-':
					{
						if (!Try_Scan_CommentStart(0)) {
							la1 = LA(1);
							if (la1 == '0')
								goto match9;
							else if (la1 == '.') {
								la2 = LA(2);
								if (la2 >= '0' && la2 <= '9')
									goto match9;
								else
									Operator();
							} else if (la1 >= '1' && la1 <= '9')
								goto match9;
							else
								Operator();
						} else
							goto match9;
					}
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					goto match9;
				case '"':
					{
						la1 = LA(1);
						if (la1 == '"') {
							la2 = LA(2);
							if (la2 == '"')
								goto match10;
							else
								goto match11;
						} else if (!(la1 == -1 || la1 == '\n' || la1 == '\r'))
							goto match11;
						else
							goto match3;
					}
				case '\'':
					{
						la1 = LA(1);
						if (la1 == '\'') {
							la2 = LA(2);
							if (la2 == '\'')
								goto match10;
							else
								goto match12;
						} else
							goto match12;
					}
				case '`':
					{
						_type = TT.BQString;
						BQString();
					}
					break;
				case ',':
					{
						_type = TT.Comma;
						Comma();
					}
					break;
				case ';':
					{
						_type = TT.Semicolon;
						Semicolon();
					}
					break;
				case '(':
					{
						_type = TT.LParen;
						LParen();
					}
					break;
				case '[':
					{
						_type = TT.LBrack;
						LBrack();
					}
					break;
				case '{':
					{
						_type = TT.LBrace;
						LBrace();
					}
					break;
				case ')':
					{
						_type = TT.RParen;
						RParen();
					}
					break;
				case ']':
					{
						_type = TT.RBrack;
						RBrack();
					}
					break;
				case '}':
					{
						_type = TT.RBrace;
						RBrace();
					}
					break;
				case '\\':
					BackslashOp();
					break;
				case '!':
				case '$':
				case '%':
				case '&':
				case '*':
				case '+':
				case ':':
				case '<':
				case '=':
				case '>':
				case '?':
				case '^':
				case '|':
				case '~':
					Operator();
					break;
				default:
					if (Token_set0.Contains(la0))
						goto match3;
					else
						goto match3;
				}
				break;
			match2:
				{
					_type = TT.Symbol;
					Symbol();
				}
				break;
			match3:
				{
					_type = TT.Id;
					Id();
				}
				break;
			match7:
				{
					_type = TT.SLComment;
					SLComment();
				}
				break;
			match8:
				{
					_type = TT.MLComment;
					MLComment();
				}
				break;
			match9:
				{
					_type = TT.Number;
					Number();
				}
				break;
			match10:
				{
					_type = TT.String;
					TQString();
				}
				break;
			match11:
				{
					_type = TT.String;
					DQString();
				}
				break;
			match12:
				{
					_type = TT.SQString;
					SQString();
				}
				break;
			match22:
				{
					_type = TT.At;
					At();
				}
			} while (false);
		}
		public void Shebang()
		{
			int la0;
			Match('#');
			Match('!');
			// Line 0: ([^\$\n\r])*
			for (;;) {
				la0 = LA0;
				if (!(la0 == -1 || la0 == '\n' || la0 == '\r'))
					Skip();
				else
					break;
			}
			// Line 0: (Newline)?
			la0 = LA0;
			if (la0 == '\n' || la0 == '\r')
				Newline();
		}
		Symbol _Comma = GSymbol.Get("#,");
		Symbol _Semicolon = GSymbol.Get("#;");
	}
}