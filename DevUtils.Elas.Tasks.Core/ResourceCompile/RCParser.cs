using System;
using System.Collections.Generic;
using DevUtils.Elas.Tasks.Core.Loyc;
using DevUtils.Elas.Tasks.Core.Loyc.Extensions;

// ReSharper disable PartialMethodWithSinglePart

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	// ReSharper disable once PartialTypeWithSinglePart
	abstract partial class RCParser : BaseParser<RCTokenType>
	{
		#region Entries

		private static readonly Dictionary<string, RCTokenType> _namedEntry = new Dictionary<string, RCTokenType>
		{
			{"menu", RCTokenType.Menu},
			{"dialog", RCTokenType.Dialog},
			{"toolbar", RCTokenType.Toolbar},
			{"dialogex", RCTokenType.Dialog},
			{"designinfo", RCTokenType.DesignInfo},
			{"versioninfo", RCTokenType.VersionInfo},
			{"accelerators", RCTokenType.Accelerators}
		};

		// ReSharper disable once UnusedMember.Local
		private static readonly Dictionary<string, RCTokenType> _resourceEntry = new Dictionary<string, RCTokenType>
		{
			{"end", RCTokenType.End},
			{"menu", RCTokenType.Menu},
			{"icon", RCTokenType.Icon},
			{"pure", RCTokenType.Pure},
			{"font", RCTokenType.Font},
			{"not", RCTokenType.NotBits},
			{"begin", RCTokenType.Begin},
			{"ltext", RCTokenType.LText},
			{"rtext", RCTokenType.RText},
			{"ctext", RCTokenType.CText},
			{"bedit", RCTokenType.BEdit},
			{"hedit", RCTokenType.HEdit},
			{"iedit", RCTokenType.IEdit},
			{"style", RCTokenType.Style},
			{"fixed", RCTokenType.Fixed},
			{"class", RCTokenType.Class},
			{"popup", RCTokenType.Popup},
			{"impure", RCTokenType.Impure},
			{"shared", RCTokenType.Shared},
			{"state3", RCTokenType.State3},
			{"control", RCTokenType.Control},
			{"preload", RCTokenType.Preload},
			{"version", RCTokenType.Version},
			{"exstyle", RCTokenType.ExStyle},
			{"caption", RCTokenType.Caption},
			{"pushbox", RCTokenType.PushBox},
			{"listbox", RCTokenType.ListBox},
			{"checkbox", RCTokenType.CheckBox},
			{"groupbox", RCTokenType.GroupBox},
			{"edittext", RCTokenType.EditText},
			{"combobox", RCTokenType.ComboBox},
			{"moveable", RCTokenType.Moveable},
			{"nonshared", RCTokenType.NonShared},
			{"separator", RCTokenType.Separator},
			{"scrollbar", RCTokenType.ScrollBar},
			{"auto3state", RCTokenType.Auto3State},
			{"pushbutton", RCTokenType.PushButton},
			{"userbutton", RCTokenType.UserButton},
			{"loadoncall", RCTokenType.LoadOnCall},
			{"discardable", RCTokenType.Discardable},
			{"radiobutton", RCTokenType.RadioButton},
			{"autocheckbox", RCTokenType.AutoCheckBox},
			{"defpushbutton", RCTokenType.DefPushButton},
			{"autoradiobutton", RCTokenType.AutoRadioButton},
			{"characteristics", RCTokenType.Characteristics},
		};

		private static readonly Dictionary<string, RCTokenType> _resourceStatement = new Dictionary<string, RCTokenType>
		{
			{"language", RCTokenType.Language},
			{"stringtable", RCTokenType.StringTable},
		};

		#endregion

		private const RCTokenType EOF = RCTokenType.EOF;

		private readonly RCLexer _lexer;

		private bool _inPP;
#pragma warning disable 649
		private Token<RCTokenType> _ret;
#pragma warning restore 649
		private Token<RCTokenType>? _lt0;
		private IDictionary<string, RCTokenType> _keywordParser;

		public RCLexer Lexer
		{
			get { return _lexer; }
		}

		protected RCParser(RCLexer lexer)
		{
			_lexer = lexer;
		}

		private bool SetRawToken(bool value)
		{
			var ret = _lexer.RawToken;
			if (ret != value)
			{
				_lt0 = null;
				_lexer.RawToken = value;
				return ret;
			}
			return ret;
		}

		private Token<RCTokenType> TryParseKeyword(Token<RCTokenType> token)
		{
			if (_keywordParser == null || (token.Type != RCTokenType.Keyword && token.Type != RCTokenType.RawKeyword))
			{
				return token;
			}

			var value = _lexer.CharSource.Substring(token).ToLowerInvariant();

			RCTokenType tt;
			if (_keywordParser.TryGetValue(value, out tt))
			{
#if DEBUG1
				Debug.WriteLine("P ({0}, {1}) {2} -> {3}", _lexer.LineNumber, _lexer.LinePosition, tt, value);
#endif
				var ret = token.ChangeType(tt);
				return ret;
			}

			return token;
		}

		private IDictionary<string, RCTokenType> SetKeywordParser(IDictionary<string, RCTokenType> dictionary)
		{
			_lt0 = null;
			var ret = _keywordParser;
			_keywordParser = dictionary;
			return ret;
		}

		partial void DesignInfo();
		partial void MenuResource();
		partial void PopupEntryBody();
		partial void DialogResource();
		partial void ToolbarResource();
		partial void VersionInfoResource();
		partial void AcceleratorResource();
		partial void SkippedDefinedResource();

		protected virtual Token<RCTokenType> OnMenuResource(Token<RCTokenType> tokenMenuId)
		{
			MenuResource();
			return _ret;
		}

		protected virtual void OnPopupEntryBody(Token<RCTokenType> tokenResourceString)
		{
			PopupEntryBody();
		}

		protected abstract void OnMenuItem(Token<RCTokenType> tokenId, Token<RCTokenType> tokenText);
		protected abstract void OnDialogControl(Token<RCTokenType> tokenControlType, Token<RCTokenType> tokenContent,
		                                        Token<RCTokenType> tokenId);

		protected virtual Token<RCTokenType> OnDialogResource(Token<RCTokenType> tokenDialogId)
		{
			DialogResource();
			return _ret;
		}

		protected virtual void OnToolbarResource()
		{
			ToolbarResource();
		}

		protected virtual void OnDesignInfo()
		{
			DesignInfo();
		}

		protected virtual void OnVersionInfoResource()
		{
			VersionInfoResource();
		}

		protected virtual void OnAcceleratorResource()
		{
			AcceleratorResource();
		}

		private void ParseNamedEntry(Token<RCTokenType> resourceStatementToken)
		{
			var kp = SetKeywordParser(_namedEntry);

			switch (LA0)
			{
				case RCTokenType.Menu:
				{
					SetRawToken(false);
					OnMenuResource(resourceStatementToken);
					SetRawToken(true);
					break;
				}
				case RCTokenType.Dialog:
				{
					SetRawToken(false);
					OnDialogResource(resourceStatementToken);
					SetRawToken(true);
					break;
				}
				case RCTokenType.Toolbar:
				{
					SetRawToken(false);
					OnToolbarResource();
					SetRawToken(true);
					break;
				}
				case RCTokenType.DesignInfo:
				{
					SetRawToken(false);
					OnDesignInfo();
					SetRawToken(true);
					break;
				}
				case RCTokenType.VersionInfo:
				{
					SetRawToken(false);
					OnVersionInfoResource();
					SetRawToken(true);
					break;
				}
				case RCTokenType.Accelerators:
				{
					SetRawToken(false);
					OnAcceleratorResource();
					SetRawToken(true);
					break;
				}
				default:
				{
					SkippedDefinedResource();
					break;
				}
			}

			SetKeywordParser(kp);
		}

		partial void StringTable();
		partial void LanguageEntry();

		protected virtual Token<RCTokenType> OnStringTable(Token<RCTokenType> stringTable)
		{
			StringTable();
			return _ret;
		}

		protected abstract void OnLanguageEntry(Token<RCTokenType> tokenPrimary, Token<RCTokenType> tokenSecondary);
		protected abstract void OnStringTableEntry(Token<RCTokenType> tokenId, Token<RCTokenType> tokenText);

		public void Process()
		{
			var kp = SetKeywordParser(_resourceStatement);
			for (;;)
			{
				var la0 = LA0;

				if (la0 == EOF)
				{
					break;
				}

				switch (la0)
				{
					case RCTokenType.Language:
					{
						SetRawToken(false);
						LanguageEntry();
						SetRawToken(true);
						break;
					}
					case RCTokenType.StringTable:
					{
						SetRawToken(false);
						OnStringTable(Match(RCTokenType.StringTable));
						SetRawToken(true);
						break;
					}
					default:
					{
						ParseNamedEntry(Match(RCTokenType.RawKeyword));
						break;
					}
				}
			}

			SetKeywordParser(kp);
		}


		protected override Token<RCTokenType> LT0
		{
			get
			{
				if (!_lt0.HasValue)
				{
					_lt0 = LT(0);
				}

				var ret = _lt0.Value;
				return ret;
			}
		}

		partial void PPParse();

		protected abstract void OnPreprocessor(Token<RCTokenType> tokenId);

		protected override Token<RCTokenType> LT(int k)
		{
			for (;;)
			{
				for (var i = 0; i <= k; ++i)
				{
					var ret = _lexer.LT(0);

#if DEBUG1
					var value = _lexer.CharSource.Substring(ret).ToLowerInvariant();
					Debug.WriteLine("L ({0}, {1}) {2} -> {3}", _lexer.LineNumber, _lexer.LinePosition, ret.Type, value);
#endif

					if (!_inPP)
					{
						switch (ret.Type)
						{
							case RCTokenType.Preprocessor:
							{
								_inPP = true;
								var prevRt = SetRawToken(false);
								var prevKp = SetKeywordParser(null);
								var preprocessorStartIndex = ret.StartIndex;
								PPParse();
								SetKeywordParser(prevKp);
								SetRawToken(prevRt);
								_inPP = false;

								OnPreprocessor(new Token<RCTokenType>(RCTokenType.Preprocessor, preprocessorStartIndex, Lexer.InputPosition));
								continue;
							}
							case RCTokenType.Newline:
							{
								_lt0 = null;
								_lexer.IgnorePP = false;
								_lexer.InputPosition = ret.EndIndex;
								continue;
							}
						}

						_lexer.IgnorePP = true;

#if DEBUG1
						value = _lexer.CharSource.Substring(ret).ToLowerInvariant();
						Debug.WriteLine("K ({0}, {1}) {2} -> {3}", _lexer.LineNumber, _lexer.LinePosition, ret.Type, value);
#endif

						ret = TryParseKeyword(ret);
					}

					return ret;
				}
			}
		}

		protected override void MoveNext()
		{
			_lexer.InputPosition = LT0.EndIndex;
			_lt0 = null;
		}

		protected override void Error(string message)
		{
			throw new Exception(message);
		}

		protected override string ToString(RCTokenType tokenType)
		{
			return tokenType.ToString();
		}

		protected override bool Equals(RCTokenType left, RCTokenType right)
		{
			return left == right;
		}
	}
}
