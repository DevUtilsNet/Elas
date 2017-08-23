using System.IO;
using DevUtils.Elas.Tasks.Core.Loyc.IO;
using DevUtils.Elas.Tasks.Core.ResourceCompile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.Elas.Tasks.Core.Tests.ResourceCompile
{
	[TestClass]
	public sealed class RCLexerTest
	{
		[TestMethod]
		public void RCLexerTestMethod1()
		{
			using (var file = File.OpenRead(@"Y:\Dev\across\arena.rc"))
			{
				var stream = new StreamCharSource(file);
				var lexer = new RCLexer(stream);

				//lexer.NextToken();

			}
		}
	}
}
