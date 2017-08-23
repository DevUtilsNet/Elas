namespace DevUtils.Elas.Tasks.Core.Xliff.Extensions
{
	/// <summary> An xliff target state extensions. </summary>
	static public class XliffTargetStateExtensions
	{
		/// <summary> An XliffTargetState extension method that query if 'targetState' is shoul be
		/// translated. </summary>
		///
		/// <param name="targetState"> The targetState to act on. </param>
		///
		/// <returns> true if should be translated, false if not. </returns>
		public static bool IsShouldBeTranslated(this XliffTargetState targetState)
		{
			var ret = targetState == XliffTargetState.New || targetState == XliffTargetState.NeedsTranslation;
			return ret;
		}

		/// <summary> An XliffTargetState extension method that query if 'targetState' is translated. </summary>
		///
		/// <param name="targetState"> The targetState to act on. </param>
		///
		/// <returns> true if translated, false if not. </returns>
		public static bool IsTranslated(this XliffTargetState targetState)
		{
			var ret = targetState == XliffTargetState.Translated;
			return ret;
		}
	}
}