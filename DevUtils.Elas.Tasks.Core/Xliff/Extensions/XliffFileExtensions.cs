
using System;
using System.Linq;

namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	static class XliffFileExtensions
	{
		public static void UpdateAbsentFlag(this XliffFile xliffFile)
		{
			foreach (var item3 in xliffFile.Units.GetAllUnits().OfType<XliffTransUnit>())
			{
				item3.Absent = !item3.IsUsed;
			}
		}

		public static void UpdateDate(this XliffFile xliffFile)
		{
			xliffFile.Date = DateTime.UtcNow;
		}
	}
}