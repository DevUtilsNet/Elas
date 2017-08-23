using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevUtils.Elas.Tasks.Core.Common;
using DevUtils.Elas.Tasks.Core.Extensions;
using DevUtils.Elas.Tasks.Core.Loyc;
using DevUtils.Elas.Tasks.Core.Loyc.Extensions;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	abstract class RCImportExportParser : RCParser
	{
		protected CultureInfo CurrentCulture { get; private set; }
		protected IEnumerable<CultureInfo> ValidSourceCultures { get; set; }

		protected bool IsValidCurrentCulture
		{
			get
			{
				var ret = CurrentCulture == null || Equals(CurrentCulture, CultureInfo.InvariantCulture) || ValidSourceCultures.Any(a => Equals(CurrentCulture, a));
				return ret;
			}
		}

		protected RCImportExportParser(RCLexer lexer) 
			: base(lexer)
		{
		}

		private static int MakeLangId(int p, int s)
		{
			var ret = (s << 10) | p;
			return ret;
		}

		protected void Parse()
		{
			CurrentCulture = null;
			Process();
		}

		protected string ExtractResourceString(Token<RCTokenType> tokenString)
		{
			var ret = Lexer.CharSource.Substring(tokenString.StartIndex + 1, tokenString.Length - 2);
			return ret;
		}

		protected string StringToId(string str)
		{
			var ret = str.GetNotRandomizedHashCode();
			return ret.ToString("X");
		}

		protected string GetDialogControlId(string id, RCTokenType controlTokeType, string content)
		{
			if (controlTokeType == RCTokenType.LText
				|| controlTokeType == RCTokenType.CText
				|| controlTokeType == RCTokenType.RText
				|| id == "IDC_STATIC")
			{
				id = id + "_" + StringToId(content);
			}

			return id;
		}

		#region Overrides of RCParser

		protected override void OnLanguageEntry(Token<RCTokenType> tokenPrimary, Token<RCTokenType> tokenSecondary)
		{
			var primary = Lexer.CharSource.Substring(tokenPrimary);
			var primaryId = WinNT.ParseLanguage(primary);
			if (primaryId == -1)
			{
				CurrentCulture = null;
			}

			var secondary = Lexer.CharSource.Substring(tokenSecondary);
			var secondaryId = WinNT.ParseSubLanguage(secondary);

			if (secondaryId == -1)
			{
				CurrentCulture = null;
			}

			var langId = MakeLangId(primaryId, secondaryId);

			CurrentCulture = langId == 0 ? CultureInfo.InvariantCulture : new CultureInfo(langId);
		}

		#endregion
	}
}
