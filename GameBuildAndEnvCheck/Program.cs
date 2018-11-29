﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			bool do_delta_compression = true;
			bool do_content_verification = false;
			if (do_delta_compression)
			{
				bool test_delta_compression = false;
				if (test_delta_compression)
				{
					ContentVerification.FilesDelta.Create(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\newbuild", @"F:\trhd.builds.pcx64\test\deltas");
					ContentVerification.FilesDelta.Apply(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\deltas", @"F:\trhd.builds.pcx64\test\patchedbuild");
				}
				else
				{
					ContentVerification.FilesDelta.Create(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7397", @"F:\trhd.builds.pcx64\7255_7397.deltas");
					ContentVerification.FilesDelta.Apply(@"F:\trhd.builds.pcx64\7255", @"F:\trhd.builds.pcx64\7255_7397.deltas", @"F:\trhd.builds.pcx64\7397.patched");
				}
			}

			if (do_content_verification)
			{
				FileVerification verifier = new FileVerification();
				verifier.Build(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
				verifier.Verify(@"F:\trhd.builds.pcx64\test\oldbuild", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
				verifier.Verify(@"F:\trhd.builds.pcx64\test\oldbuild.modified", @"F:\trhd.builds.pcx64\test\oldbuild.fcv");
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form1());
			}
		}
	}
}