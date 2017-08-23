using System.Collections.Generic;

namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	/// <summary> An xliff base extensions. </summary>
	public static class XliffBaseExtensions
	{
		/// <summary> Enumerates select parent in this collection. </summary>
		///
		/// <param name="xliffBase"> The xliffBase to act on. </param>
		///
		/// <returns> An enumerator that allows foreach to be used to process select parent in this
		/// collection. </returns>
		public static IEnumerable<XliffBase> SelectParent(this XliffBase xliffBase)
		{
			while (xliffBase != null)
			{
				if (xliffBase.Parent != null)
				{
					yield return xliffBase.Parent;
				}
				xliffBase = xliffBase.Parent;
			}
		}
	}
}