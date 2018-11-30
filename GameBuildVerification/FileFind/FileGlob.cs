using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ColoredConsole;

namespace ContentVerification
{
	public static class HgxFile
	{
		public static void GlobSimple(string _dir, string _arg, List<FileData> _out_filenames, FileFilter _ignored)
		{
			if (Directory.Exists(_dir))
			{
				if (!_dir.EndsWith("\\"))
					_dir += "\\";

				foreach (FileData fd in FindFiles.EnumerateFiles(_dir, _arg, _ignored, SearchOption.AllDirectories))
				{
					string filepath = fd.Path.Substring(_dir.Length);
					if (!_ignored.IsFileIgnored(filepath.ToLower()))
						_out_filenames.Add(fd);
				}
			}
		}

		public static bool Glob(string _basepath, string _arg, List<FileData> _out_filenames, FileFilter filter)
		{
			_basepath = _basepath.EnsureEndsWith("\\");
			if (String.IsNullOrEmpty(_arg))
				_arg = ".";

			// @_arg will be used as a Wildcard
			// Start the search from the current work-directory
			Wildcard wildcard = new Wildcard(_arg.ToLower(), true);
			foreach (FileData fd in FindFiles.EnumerateFiles(_basepath, "*.*", filter, SearchOption.AllDirectories))
			{
				string filepath = fd.Path.Substring(_basepath.Length);
				if (!filter.IsFileIgnored(filepath.ToLower()))
				{
					if (wildcard.IsMatch(filepath))
					{
						_out_filenames.Add(fd);
					}
				}
			}

			wildcard.ForEachDirectMatch(delegate(string _filepath)
			{
				if (File.Exists(_filepath))
				{
					Console2.WriteLineWithColor(ConsoleColor.Yellow, "warning: file \"{0}\" exists but is ignored by hgx", _filepath);
				}
				else
				{
					Console2.WriteLineWithColor(ConsoleColor.Yellow, "warning: file \"{0}\" doesn't exist and is ignored by hgx", _filepath);
				}
			});

			return true;
		}

	}
}