using System.Diagnostics;
using System.IO;
using DevUtils.Elas.Tasks.Core.Loyc;
using DevUtils.Elas.Tasks.Core.Loyc.IO;
using DevUtils.Elas.Tasks.Core.ResourceCompile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.Elas.Tasks.Core.Tests.ResourceCompile
{
	[TestClass]
	public sealed class RCParserTest
	{
		private ICharSource _charSource;

		//class TestRCParser : RCParser
		//{
		//	public TestRCParser(RCLexer lexer) : base(lexer)
		//	{
		//	}

		//	#region Overrides of RCParser

		//	protected override void OnLanguageEntry(Token<RCTokenType> tokenPrimary, Token<RCTokenType> tokenSecondary)
		//	{
		//		//Debug.WriteLine("Language {0}, {1}", primary, secondary);
		//	}

		//	protected override void OnStringTableEntry(Token<RCTokenType> tokenId, Token<RCTokenType> tokenText)
		//	{
		//		//Debug.WriteLine("StringTableEntry {0}, {1}", id, text);
		//	}

		//	protected override void OnPreprocessor(Token<RCTokenType> tokenId)
		//	{
		//	}

		//	#endregion
		//}

		[TestMethod]
		public void RCParserTestMethod1()
		{
			//using (var file = File.OpenRead(@"Y:\ELAS\Dev\TestMFCApplication\TestResource.rc"))
			using (var file = File.OpenRead(@"Y:\Dev\across\arena1.rc"))
			{
				var stream = new StreamCharSource(file);
				_charSource = stream;
				var lexer = new RCLexer(stream);

				//var parser = new TestRCParser(lexer);

				//parser.Process();
			}
		}
	}
}
