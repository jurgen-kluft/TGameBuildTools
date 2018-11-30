#ifdef CCOREDLL_EXPORT
#define CCOREDLL_API __declspec(dllexport) 
#else
#define CCOREDLL_API __declspec(dllimport) 
#endif

extern "C" 
{
	CCOREDLL_API void Skein512_256(char* src, int src_length, char* hash);
	CCOREDLL_API void Skein512_160(char* src, int src_length, char* hash);

	CCOREDLL_API int LZ4_StateSize();
	CCOREDLL_API int LZ4_Compress(char* state, char* src, int src_length, char* dst, int dst_length);
	CCOREDLL_API int LZ4_StateSizeHC();
	CCOREDLL_API int LZ4_CompressHC(char* state, char* src, int src_length, char* dst, int dst_length);
	CCOREDLL_API int LZ4_Decompress(char* src, int src_length, char* dst, int dst_length);
}