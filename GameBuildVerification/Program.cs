using System;
using CommandLineParser.Arguments;
using ContentVerification;

namespace GameBuildTools
{
	static class Program
	{
		class CmdLineArgs
		{
			[SwitchArgument('s', "version", false, Description = "Show version", Optional = true)]
			public bool version { get; set; }

			[SwitchArgument('v', "verbose", false, Description = "Set verbose (true/false)", Optional = true)]
			public bool verbose { get; set; }

			[ValueArgument(typeof(string), 'c', "cmd", Description = "Create a verification signature for a folder / Verify a folder using a verification signature", Optional = true)]
			public string cmd { get; set; }

			[ValueArgument(typeof(string), 'f', "folder", Description = "Set the path to the folder that should be used/verified", Optional = true)]
			public string folder { get; set; }

			[ValueArgument(typeof(string), 'o', "output", Description = "The file-path to the verification signature file.", Optional = true)]
			public string signature { get; set; }
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			CommandLineParser.CommandLineParser cmdline = new CommandLineParser.CommandLineParser();
			try
			{
				CmdLineArgs c = new CmdLineArgs();
				cmdline.ExtractArgumentAttributes(c);
				cmdline.ParseCommandLine(args);
				cmdline.ShowUsageFooter = "example: GameBuildVerification -c create -f D:\\build.yesterday -o D:\\build.today.patch.gbv";

				if (c.version)
				{
					Console.WriteLine("v1.0.0");
				}
				else
				{
					FileVerification verifier = new FileVerification();
					verifier.Verbose = c.verbose;

					if (c.cmd == "create")
					{
						verifier.Build(c.folder, c.signature);
					}
					else if (c.cmd == "verify")
					{
						verifier.Verify(c.folder, c.signature);
					}
					else
					{
						cmdline.ShowUsageHeader = "This is how to use GameBuildVerification";
						cmdline.ShowUsage();
					}
				}
			}
			catch (CommandLineParser.Exceptions.CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdline.ShowUsage();
			}
		}
	}
}