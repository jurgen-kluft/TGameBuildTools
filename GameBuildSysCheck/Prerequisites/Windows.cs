using System;
using Microsoft.Win32;

namespace GameBuildTools
{
	public static class Windows
	{
		private static string HKLM_GetString(string key, string value)
		{
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
				return registryKey?.GetValue(value).ToString() ?? String.Empty;
			}
			catch
			{
				return String.Empty;
			}
		}

		public static string GetVersion()
		{
			string osArchitecture;
			try
			{
				osArchitecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
			}
			catch (Exception)
			{
				osArchitecture = "32/64-bit (Undetermined)";
			}
			string productName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
			string csdVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
			string currentBuild = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild");
			if (!string.IsNullOrEmpty(productName))
			{
				return $"{productName}{(!string.IsNullOrEmpty(csdVersion) ? " " + csdVersion : String.Empty)} {osArchitecture} (OS Build {currentBuild})";
			}
			return String.Empty;
		}
	}
}
