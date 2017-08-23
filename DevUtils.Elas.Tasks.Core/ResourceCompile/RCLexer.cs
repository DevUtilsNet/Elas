using System;
using DevUtils.Elas.Tasks.Core.Loyc;

namespace DevUtils.Elas.Tasks.Core.ResourceCompile
{
	// ReSharper disable once PartialTypeWithSinglePart
	sealed partial class RCLexer : BaseLexer
	{
		private int _endPosition;
		private RCTokenType _type;
		private int _startPosition;
		private int _nestedTokenCount;

		public bool RawToken { get; set; }
		public bool IgnorePP { get; set; }

		public RCLexer()
		{
			RawToken = true;
		}

		public RCLexer(ICharSource source)
			: base(source)
		{
			RawToken = true;
		}

		// ReSharper disable once UnusedMember.Local
		private void BeginToken()
		{
			if (_nestedTokenCount == 0)
			{
				_startPosition = InputPosition;
			}

			++_nestedTokenCount;
		}

		// ReSharper disable once UnusedMember.Local
		private void EndToken(RCTokenType tt)
		{
			_type = tt;
			_endPosition = InputPosition;

			if (_nestedTokenCount == 0)
			{
				throw new OverflowException("_nestedTokenCount");
			}
			--_nestedTokenCount;
		}

		// ReSharper disable once PartialMethodWithSinglePart
		partial void Token();

		public Token<RCTokenType> LT(int k)
		{
			var ip = InputPosition;
			for (var i = 0; i <= k; ++i)
			{
				for (; ; )
				{
					Token();

					if (_type != RCTokenType.SpacesOrComments)
					{
						break;
					}
				}
			}

			InputPosition = ip;
			var ret = new Token<RCTokenType>(_type, _startPosition, _endPosition);

			return ret;
		}

		protected override void Error(int lookaheadIndex, string message)
		{
			throw new Exception(message);
		}
	}
}
