using System;

namespace DevUtils.Elas.Tasks.Core.Loyc.Extensions
{
	static class CharSourceExtensions
	{
		public static string Substring(this ICharSource charSource, int startIndex, int length)
		{
			var buffer = new char[length];
			length = charSource.Read(buffer, startIndex, 0, length);
			var ret = new String(buffer, 0, length);
			return ret;
		}

		public static string Substring<TT>(this ICharSource charSource, Token<TT> token) where TT : struct
		{
			var ret = charSource.Substring(token.StartIndex, token.Length);
			return ret;
		}

		public static int Read(this ICharSource charSource, char[] buffer, int dataOffset)
		{
			var ret = charSource.Read(buffer, dataOffset, 0, buffer.Length);
			return ret;
		}
	}
}
