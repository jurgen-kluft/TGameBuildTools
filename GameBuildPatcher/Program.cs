using System;
using CommandLineParser.Arguments;

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

			[ValueArgument(typeof(string), 'c', "cmd", Description = "Apply a patch (ref+patch = build), Create a patch (ref+build = patch)", Optional = true)]
			public string cmd { get; set; }

			[ValueArgument(typeof(string), 'r', "refpath", Description = "Set the reference path, this is the reference build", Optional = true)]
			public string refpath { get; set; }

			[ValueArgument(typeof(string), 'b', "buildpath", Description = "Set the build path, this is the path we will make a patch for (create) or the path we will write to (apply)", Optional = true)]
			public string buildpath { get; set; }

			[ValueArgument(typeof(string), 'p', "patchpath", Description = "Set the path to where the patch will be created to or applied from.", Optional = true)]
			public string patchpath { get; set; }
		}

		[STAThread]
		static void Main(string[] args)
		{
			CommandLineParser.CommandLineParser cmdline = new CommandLineParser.CommandLineParser();
			try
			{
				// We should see if we can add 'grouping' into the commandline parameters instead of Optional!

				CmdLineArgs c = new CmdLineArgs();
				cmdline.ExtractArgumentAttributes(c);
				cmdline.ParseCommandLine(args);
				cmdline.ShowUsageFooter = "example: GameBuildPatcher -c create -r D:\\build.yesterday -b D:\\build.today -p D:\\build.today.patch";

				if (c.version)
				{
					Console.WriteLine("v1.0.0");
				}
				else
				{
					Patcher.Verbose = c.verbose;

					c.cmd = c.cmd.ToLower();
					if (c.cmd == "create")
					{
						Patcher.Create(c.refpath, c.buildpath, c.patchpath);
					}
					else if (c.cmd == "apply")
					{
						Patcher.Apply(c.refpath, c.patchpath, c.buildpath);
					}
					else
					{
						cmdline.ShowUsageHeader = "This is how to use GameBuildPatcher";
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
