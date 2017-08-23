namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	/// <summary> An xliff target extensions. </summary>
	public static class XliffTargetExtensions
	{
		/// <summary> An XliffTarget extension method that query if 'target' is shoul be translated. </summary>
		///
		/// <param name="target"> The target to act on. </param>
		///
		/// <returns> true if should be translated, false if not. </returns>
		public static bool IsShouldBeTranslated(this XliffTarget target)
		{
			var ret = target != null
			          && target.State.HasValue
			          && target.State.Value.IsShouldBeTranslated();
			return ret;
		}

		/// <summary> An XliffTarget extension method that query if 'target' is translated. </summary>
		///
		/// <param name="target"> The target to act on. </param>
		///
		/// <returns> true if translated, false if not. </returns>
		public static bool IsTranslated(this XliffTarget target)
		{
			var ret = target != null
			          && target.State.HasValue
			          && target.Content != null
			          && target.State.Value.IsTranslated();
			return ret;
		}
	}
}