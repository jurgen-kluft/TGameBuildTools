using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentVerification
{
	public static class FileChunks
	{
		public static int DefaultChunkSize { get { return 1024 * 1024; } }
		public static int HeaderSize { get { return 64; } }
		public static int DefaultChunkSizeAlignment { get { return 64; } }
		public static Int64 InvalidOffset { get { return Int64.MaxValue; } }

		public static bool IsSane(int length, int lengthuc, Int64 offset)
		{
			bool is_sane_chunk = (length >= 0 && length <= FileChunks.DefaultChunkSize);
			is_sane_chunk = is_sane_chunk && (lengthuc >= length);
			is_sane_chunk = is_sane_chunk && (IsValidOffset(offset) || (offset == FileChunks.InvalidOffset));
			return is_sane_chunk;
		}

		public static bool IsValidOffset(Int64 offset)
		{
			return (offset >= 0 && offset < ((Int64)256 * 1024 * 1024 * 1024));
		}

		public static void Test()
		{
			Int64 filesize1 = (Int64)1024 * 1024 * 1024 * 4 + FileChunks.DefaultChunkSize + 2048;
			int chunklen1 = ComputeLength(16 * 1024, filesize1);
			int chunklen2 = ComputeLength(16 * 1024 + 1, filesize1);
			Debug.Assert(chunklen1 == FileChunks.DefaultChunkSize);
			Debug.Assert(chunklen2 == 2048);
			Int64 chunkoff1, chunkoff2;
			ComputeLengthAndOffset(16 * 1024, filesize1, out chunkoff1, out chunklen1);
			ComputeLengthAndOffset(16 * 1024 + 1, filesize1, out chunkoff2, out chunklen2);
			Debug.Assert(chunklen1 == FileChunks.DefaultChunkSize);
			Debug.Assert(chunkoff1 == ((Int64)1024 * 1024 * 1024 * 4));
			Debug.Assert(chunklen2 == 2048);
			Debug.Assert(chunkoff2 == (((Int64)1024 * 1024 * 1024 * 4) + FileChunks.DefaultChunkSize));
		}

		public static int ComputeAlignedLength(int length)
		{
			int aligned = (length + (FileChunks.DefaultChunkSizeAlignment - 1)) & ~(FileChunks.DefaultChunkSizeAlignment - 1);
			return aligned;
		}

		public static void ComputeNumberOfChunks(Int64 _file_size, out int _out_number_of_chunks)
		{
			Int64 n = (_file_size + (Int64)(DefaultChunkSize - 1)) / (Int64)DefaultChunkSize;
			_out_number_of_chunks = (int)n;
		}

		public static int ComputeLength(int _chunk_index, Int64 _file_size)
		{
			Int64 out_chunk_offset = DefaultChunkSize;
			out_chunk_offset *= _chunk_index;

			if (out_chunk_offset >= _file_size)
				return 0;

			int out_chunk_length = DefaultChunkSize;
			if ((out_chunk_offset + out_chunk_length) > _file_size)
				out_chunk_length = (int)(_file_size - out_chunk_offset);
			return out_chunk_length;
		}

		public static bool ComputeLengthAndOffset(int _chunk_index, Int64 _file_size, out Int64 _out_chunk_offset, out int _out_chunk_length)
		{
			_out_chunk_offset = DefaultChunkSize;
			_out_chunk_offset *= _chunk_index;
			_out_chunk_length = 0;
			if (_out_chunk_offset >= _file_size)
				return false;
			_out_chunk_length = DefaultChunkSize;
			if ((_out_chunk_offset + _out_chunk_length) > _file_size)
				_out_chunk_length = (int)(_file_size - _out_chunk_offset);
			return true;
		}
	}

}

