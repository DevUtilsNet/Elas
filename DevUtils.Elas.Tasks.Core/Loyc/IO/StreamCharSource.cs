using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevUtils.Elas.Tasks.Core.Loyc.IO
{

	/// <summary> A stream character source. This class cannot be inherited. </summary>
	public sealed class StreamCharSource : ICharSource
	{
		struct BufferBlock
		{
			public long BytesOffset;
			public int BytesLength;
		}
		private const int CharBufferLength = 0x1000;
		private readonly Stream _stream;			// stream from which data is read

		//private const int MaxSeqSize = 8;
		//private const int DefaultBufSize = 2048 + MaxSeqSize - 1;

		private Decoder _decoder;
		private Encoding _encoding;
		private byte[] _byteBuffer;			// input buffer
		private char[][] _charBuffers;	// character buffers
		private int _currentBufferIndex;
		private int[] _charBuffersLength;
		private int[] _charBuffersOffset;

		/// <summary>A sorted list of mappings between byte positions and character 
		/// indexes. In each Pair(of A,B), A is the character index and B is the byte 
		/// index. This list is built on-demand.
		/// </summary>
		private readonly List<BufferBlock> _blkOffsets = new List<BufferBlock>();

		/// <summary> Constructor. </summary>
		///
		/// <param name="stream"> The stream. </param>
		public StreamCharSource(Stream stream)
			: this(stream, null)
		{
		}

		/// <summary> Constructor. </summary>
		///
		/// <exception cref="ArgumentException">	Thrown when one or more arguments have unsupported or
		/// 																			illegal values. </exception>
		///
		/// <param name="stream">	  The stream. </param>
		/// <param name="encoding"> The encoding. </param>
		public StreamCharSource(Stream stream, Encoding encoding)
		{
			_encoding = encoding;
			if (_encoding != null)
			{
				_decoder = _encoding.GetDecoder();
			}

			if (!stream.CanSeek)
			{
				throw new ArgumentException("stream does not support seeking.");
			}

			_stream = stream;
		}

		private void DetectEncoding()
		{
			var encodings = Encoding.GetEncodings().Select(s => s.GetEncoding()).ToArray();

			var maxPreamble = encodings.Select(s => s.GetPreamble().Length).Max();

			var savePosition = _stream.Position;
			var byteBuffer = new byte[maxPreamble];
			var byteBufferLength = _stream.Read(byteBuffer, 0, byteBuffer.Length);

			_encoding = encodings.FirstOrDefault(f =>
			{
				var pream = f.GetPreamble();

				return pream.Length > 0 && pream.SequenceEqual(byteBuffer.Take(Math.Min(pream.Length, byteBufferLength)));
			}) ?? Encoding.ASCII;

			_decoder = _encoding.GetDecoder();

			_stream.Position = savePosition + _encoding.GetPreamble().Length;
		}

		/// <summary> Reads. </summary>
		///
		/// <param name="buffer">		  The buffer. </param>
		/// <param name="dataOffset"> The data offset. </param>
		/// <param name="index">		  Zero-based index of the. </param>
		/// <param name="count">		  Number of. </param>
		///
		/// <returns> An int. </returns>
		public int Read(char[] buffer, int dataOffset, int index, int count)
		{
			var ret = 0;

			if (_charBuffers == null)
			{
				CreateBuffers();
			}

			for (;;)
			{
				var found = true;
				while (found)
				{
					found = false;
					for (var i = 0; i < _charBuffers.Length; ++i)
					{
						var buffOffset = _charBuffersOffset[i];
						var buffLength = _charBuffersLength[i];
						if (buffOffset <= dataOffset && dataOffset < buffOffset + buffLength)
						{
							var toCopy = Math.Min(buffLength - (dataOffset - buffOffset), count);
							Array.Copy(_charBuffers[i], dataOffset - buffOffset, buffer, index, toCopy);
							count -= toCopy;
							ret += toCopy;
							if (count == 0)
							{
								return ret;
							}
							dataOffset += toCopy;
							index += toCopy;
							found = true;
						}
					}
				}

				++_currentBufferIndex;

				if (!LoadBuffer(dataOffset))
				{
					return ret;
				}
			}
		}

		private bool LoadBuffer(int dataOffset)
		{
			var buffIndex = _currentBufferIndex % _charBuffers.Length;

			while (true)
			{
				var blockIndex = GetBlockIndex(dataOffset);

				BufferBlock blkOffset;
				if (blockIndex >= _blkOffsets.Count)
				{
					blockIndex = _blkOffsets.Count - 1;
					blkOffset = _blkOffsets[blockIndex];
					if (blkOffset.BytesLength == 0)
					{
						_stream.Position = _blkOffsets[blockIndex].BytesOffset;
					}
					else
					{
						blkOffset = new BufferBlock {BytesOffset = blkOffset.BytesOffset + blkOffset.BytesLength};
						_blkOffsets.Add(blkOffset);
						++blockIndex;
					}
				}
				else
				{
					blkOffset = _blkOffsets[blockIndex];
				}

				_stream.Position = blkOffset.BytesOffset;

				_decoder.Reset();

				var byteBufferLength = _stream.Read(_byteBuffer, 0, _byteBuffer.Length);
				if (byteBufferLength == 0)
				{
					return false;
				}

				int bytesUsed;
				int charsUsed;
				bool completed;
				_decoder.Convert(_byteBuffer, 0, byteBufferLength, _charBuffers[buffIndex], 0, _charBuffers[buffIndex].Length, true,
					out bytesUsed, out charsUsed, out completed);

				var charOffest = blockIndex * CharBufferLength;
				_charBuffersOffset[buffIndex] = charOffest;
				_charBuffersLength[buffIndex] = charsUsed;

				if (blkOffset.BytesLength == 0)
				{
					blkOffset.BytesLength = bytesUsed;
					_blkOffsets[blockIndex] = blkOffset;
				}

				if (charOffest <= dataOffset && dataOffset < charOffest + charsUsed)
				{
					break;
				}

				if (byteBufferLength < _byteBuffer.Length)
				{
					return false;
				}
			}
			return true;
		}

		private void CreateBuffers()
		{
			if (_decoder == null)
			{
				DetectEncoding();
			}

			var maxBytes = _encoding.GetMaxByteCount(CharBufferLength);
			_byteBuffer = new byte[maxBytes];

			_blkOffsets.Add(new BufferBlock {BytesOffset = _stream.Position});

			_charBuffers = new[] { new char[CharBufferLength], new char[CharBufferLength], new char[CharBufferLength] };
			_charBuffersOffset = new int[_charBuffers.Length];
			_charBuffersLength = new int[_charBuffers.Length];
		}

		private static int GetBlockIndex(int charIndex)
		{
			var ret = charIndex / CharBufferLength;
			return ret;
		}
	}
}