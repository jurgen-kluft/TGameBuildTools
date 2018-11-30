using System;
using System.Collections.Generic;
using System.Collections;

namespace GameBuildTools
{
	public static class EnvVars
	{
		public static void Get(Dictionary<string, string> envvars)
		{
			var compName = System.Environment.GetEnvironmentVariables()["COMPUTERNAME"];
			envvars.Add("ComputerName", compName.ToString());

			foreach (DictionaryEntry e in System.Environment.GetEnvironmentVariables())
			{
				envvars.Add(e.Key.ToString(), e.Value.ToString());
			}
		}

		public static void Print()
		{
			Dictionary<string, string> envvars = new Dictionary<string, string>();
			Get(envvars);
			foreach (var e in envvars)
			{
				Console.WriteLine(e.Key + " = " + e.Value);
			}
		}
	}
}