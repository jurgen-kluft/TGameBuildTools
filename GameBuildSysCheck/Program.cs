using System;
using System.Windows.Forms;

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
			EnvVars.Print();
			InstalledSoftware.Print();
			WindowsSDK.Print();
		}

	}
}
