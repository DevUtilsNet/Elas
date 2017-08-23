namespace DevUtils.Elas.Tasks.Core.Loyc
{
	/// <summary> Interface for character source. </summary>
	public interface ICharSource
	{
		/// <summary> Reads. </summary>
		///
		/// <param name="buffer">		  The buffer. </param>
		/// <param name="dataOffset"> The data offset. </param>
		/// <param name="index">		  Zero-based index of the. </param>
		/// <param name="count">		  Number of. </param>
		///
		/// <returns> An int. </returns>
		int Read(char[] buffer, int dataOffset, int index, int count);
	}
}