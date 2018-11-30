using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace GameBuildAndEnvCheck
{
	public static class WindowsSDK
	{
		public static void Get(Dictionary<string, string> sdks)
		{
			string winkitskey = @"SOFTWARE\WOW6432Node\Microsoft\Windows Kits\Installed Roots";
			using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(winkitskey))
			{
				foreach (string vn in rk.GetValueNames())
				{
					if (vn.StartsWith("KitsRoot"))
					{
						string sdklocation = rk.GetValue(vn).ToString();
						string[] folders = sdklocation.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
						string sdkver = folders[folders.Length - 1];
						sdks.Add(sdkver, sdklocation);
					}
				}
			}

			string sdk10location;
			if (sdks.TryGetValue("10", out sdk10location))
			{
				AddAllVersionsFromFolder(sdk10location, "include", sdks);
			}

			using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(winkitskey))
			{
				foreach (string skName in rk.GetSubKeyNames())
				{
					if (skName.StartsWith("10."))
					{
						string sdklocation;
						if (sdks.TryGetValue("10", out sdklocation))
						{
							if (sdks.ContainsKey(skName) == false)
							{
								sdks.Add(skName, sdklocation);
							}
						}
					}
				}
			}
		}

		private static void AddAllVersionsFromFolder(string sdkpath, string subpath, Dictionary<string, string> sdks)
		{
			DirectoryInfo dirinfo = new DirectoryInfo(Path.Combine(sdkpath, subpath));
			foreach(DirectoryInfo di in dirinfo.EnumerateDirectories())
			{
				sdks.Add(di.Name, sdkpath);
			}
		}

		public static void Print()
		{
			Dictionary<string, string> sdks = new Dictionary<string, string>();
			Get(sdks);
			foreach(var sdk in sdks)
			{
				Console.WriteLine("Windows SDK {0} = {1}", sdk.Key, sdk.Value);
			}
		}

	}
}