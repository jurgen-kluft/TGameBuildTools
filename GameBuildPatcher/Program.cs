using System;

namespace GameBuildTools
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			bool test_delta_compression = false;
			if (test_delta_compression)
			{
				FilesDelta.Create(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\newbuild", @"F:\trhd.builds.pcx64\test\deltas");
				FilesDelta.Apply(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\deltas", @"F:\trhd.builds.pcx64\test\patchedbuild");
			}
			else
			{
				FilesDelta.Create(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7397", @"F:\trhd.builds.pcx64\7255_7397.deltas");
				FilesDelta.Apply(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7255_7397.deltas", @"F:\trhd.builds.pcx64\7397.patched");
			}
		}

	}
}
