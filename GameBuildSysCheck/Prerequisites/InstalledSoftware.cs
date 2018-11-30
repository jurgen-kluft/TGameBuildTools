using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace GameBuildTools
{
	public static class InstalledSoftware
	{
		public static void Get(Dictionary<string, string> software, Dictionary<string, string> location)
		{
			string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
			using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(uninstallKey))
			{
				foreach (string skName in rk.GetSubKeyNames())
				{
					using (RegistryKey sk = rk.OpenSubKey(skName))
					{
						try
						{
							string softwareName = sk.GetValue("DisplayName").ToString();
							string installLocation = sk.GetValue("InstallLocation").ToString();
							software.Add(skName, softwareName);
							location.Add(skName, installLocation);
						}
						catch (Exception)
						{ }
					}
				}
			}
		}

		public static void Print()
		{
			Dictionary<string, string> software = new Dictionary<string, string>();
			Dictionary<string, string> location = new Dictionary<string, string>();
			Get(software, location);
			foreach(var kvs in software)
			{
				Console.WriteLine("{0} = {1}", kvs.Key, kvs.Value);
			}
		}
	}
}