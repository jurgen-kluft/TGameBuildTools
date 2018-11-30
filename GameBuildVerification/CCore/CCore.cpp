#include "CCore.h"
#include "skein\skein.h"
#include "lz4\lz4.h"
#include "lz4\lz4hc.h"

extern "C" {
	void Skein512_256(char* src, int src_length, char* hash)
	{
		Skein_512_Ctxt_t c;
		Skein_512_Init(&c, 256);
		Skein_512_Update(&c, (const u08b_t*)src, src_length);
		Skein_512_Final(&c, (u08b_t*)hash);
	}

	void Skein512_160(char* src, int src_length, char* hash)
	{
		Skein_512_Ctxt_t c;
		Skein_512_Init(&c, 160);
		Skein_512_Update(&c, (const u08b_t*)src, src_length);
		Skein_512_Final(&c, (u08b_t*)hash);
	}

	int LZ4_StateSizeHC()
	{
		int size = LZ4_sizeofStateHC();
		return size;
	}

	int LZ4_CompressHC(char* state, char* src, int src_length, char* dst, int dst_length)
	{
		int r = LZ4_compressHC2_limitedOutput_withStateHC(state, (const char*)src, (char*)dst, src_length, dst_length, 9);
		return r;
	}

	int LZ4_StateSize()
	{
		int size = LZ4_sizeofState();
		return size;
	}

	int LZ4_Compress(char* state, char* src, int src_length, char* dst, int dst_length)
	{
		int r = LZ4_compress_limitedOutput_withState(state, (const char*)src, (char*)dst, src_length, dst_length);
		return r;
	}

	int LZ4_Decompress(char* src, int src_length, char* dst, int dst_length)
	{
		int r = LZ4_decompress_safe((const char*)src, (char*)dst, src_length, dst_length);
		return r;
	}
}