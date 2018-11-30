using System;
using System.Windows.Forms;
using ContentVerification;

namespace GameBuildAndEnvCheck
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			EnvVars.Print();
			InstalledSoftware.Print();
			WindowsSDK.Print();

			bool do_delta_compression = false;
			if (do_delta_compression)
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

			bool do_content_verification = false;
			if (do_content_verification)
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

			bool do_gui = false;
			if (do_gui)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
		}



	}
}
