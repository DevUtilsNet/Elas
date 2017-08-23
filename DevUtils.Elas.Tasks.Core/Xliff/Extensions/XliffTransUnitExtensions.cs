namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	static class XliffTransUnitExtensions
	{
		public static bool IsTranslated(this XliffTransUnit unit)
		{
			var ret = !unit.NotTranslate
			          && unit.Target.IsTranslated();
			return ret;
		}
	}
}