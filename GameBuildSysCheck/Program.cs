using System;
using System.IO;

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
			//EnvVars.Print();
			//InstalledSoftware.Print();
			WindowsSDK.Print();

			string winver = Windows.GetVersion();
			Console.WriteLine(winver);
		} 

	}
}
