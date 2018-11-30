using System;
using ContentVerification;

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
			bool test_content_verification = false;
			if (test_content_verification)
			{
				FileVerification verifier = new FileVerification();
				verifier.Build(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
				verifier.Verify(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
				verifier.Verify(@"F:\trhd.builds.pcx64\test\oldbuild.modified", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
			}
			else
			{
				FileVerification verifier = new FileVerification();
				//verifier.Build(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7255.fcv");
				verifier.Verify(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7255.fcv");
			}
		}
	}
}
