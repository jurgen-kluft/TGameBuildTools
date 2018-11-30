using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ContentVerification
{
	public class CCore
	{
#if DEBUG
		public const string CCORE_DLLNAME = "CCoreD";
#else
		public const string CCORE_DLLNAME = "CCore";
#endif

		[DllImport(CCORE_DLLNAME, EntryPoint = "Skein512_256")]
		public static extern void SkeinHash(byte[] src, int length, byte[] hash);

		[DllImport(CCORE_DLLNAME, EntryPoint = "LZ4_StateSize")]
		public static extern int LZ4_StateSize();

		[DllImport(CCORE_DLLNAME, EntryPoint = "LZ4_Compress")]
		public static extern int LZ4_Compress(byte[] state, byte[] src, int src_length, byte[] dst, int dst_max_length);

		[DllImport(CCORE_DLLNAME, EntryPoint = "LZ4_StateSizeHC")]
		public static extern int LZ4_StateSizeHC();

		[DllImport(CCORE_DLLNAME, EntryPoint = "LZ4_CompressHC")]
		public static extern int LZ4_CompressHC(byte[] state, byte[] src, int src_length, byte[] dst, int dst_max_length);

		[DllImport(CCORE_DLLNAME, EntryPoint = "LZ4_Decompress")]
		public static extern int LZ4_Decompress(byte[] src, int src_length, byte[] dst, int dst_max_length);

		public static void TestHash()
		{
			byte[] src = new byte[1024];
			int length = src.Length;
			byte[] hashbytes = new byte[32];
			CCore.SkeinHash(src, length, hashbytes);
			Hash256 hash = Hash256.ConstructCopy(hashbytes);

			Skein256 skein = new Skein256();
			skein.Initialize();
			Hash256 hash2 = Hash256.ConstructTake(skein.ComputeHash(src));

			Debug.Assert(hash.Compare(hash2) == 0);
		}

		public static void TestCompress()
		{
			Stopwatch timer = new Stopwatch();

			byte[] testdata = new byte[250000];
			for (int i=0; i<testdata.Length; ++i)
			{
				testdata[i] = (byte)(i*2);
			}

			int lz4_state_size = LZ4_StateSizeHC();
			byte[] lz4_state = new byte[lz4_state_size];

			byte[] testdata_compressed_1 = new byte[256000];
			int testdata_compressed_1_size = LZ4s.LZ4Codec.Encode64(testdata, 0, testdata.Length, testdata_compressed_1, 0, -1);
			timer.Reset();
			timer.Start();
			for (int i = 0; i < 2; ++i)
			{
				testdata_compressed_1_size = LZ4s.LZ4Codec.Encode64(testdata, 0, testdata.Length, testdata_compressed_1, 0, -1);
			}
			timer.Stop();
			///Console.WriteLine("info: managed LZ4 took {0}", timer.Elapsed);

			byte[] testdata_compressed_2 = new byte[256000];
			int testdata_compressed_2_size = LZ4_Compress(lz4_state, testdata, testdata.Length, testdata_compressed_2, testdata_compressed_2.Length);
			timer.Reset();
			timer.Start();
			for (int i = 0; i < 2; ++i)
			{
				testdata_compressed_2_size = LZ4_CompressHC(lz4_state, testdata, testdata.Length, testdata_compressed_2, testdata_compressed_2.Length);
			}
			timer.Stop();
			///Console.WriteLine("info: native LZ4 took {0}", timer.Elapsed);

			Debug.Assert(testdata_compressed_1_size == testdata_compressed_2_size);
			for (int i=0; i<testdata_compressed_2_size; ++i)
			{
				Debug.Assert(testdata_compressed_1[i] == testdata_compressed_2[i]);
			}


		}

	}
}
