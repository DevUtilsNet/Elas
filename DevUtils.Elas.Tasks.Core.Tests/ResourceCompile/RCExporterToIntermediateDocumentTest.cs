using System.Globalization;
using DevUtils.Elas.Tasks.Core.ResourceCompile;
using DevUtils.Elas.Tasks.Core.Xliff;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtils.Elas.Tasks.Core.Tests.ResourceCompile
{
	[TestClass]
	public sealed class RCExporterToIntermediateDocumentTest
	{
		[TestMethod]
		public void RCExporterToIntermediateDocumentTestMethod1()
		{
			var e = new RCExporterToIntermediateDocument();

			var doc = new XliffDocument();
			//e.Export(@"Y:\Dev\across\arena1.rc", doc, new CultureInfo("en-US"), new [] {new CultureInfo("de-DE")} );
		}
	}
}
