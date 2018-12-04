using System;
using System.IO;
using System.Collections.Generic;
using ColoredConsole;

namespace ContentVerification
{
	public class FileVerification
	{
		public bool Verbose { get; set; }

		public void Build(string rootdir, string output_filepath)
		{
			rootdir = rootdir.EnsureEndsWith("\\");

			FileFilter filter = new FileFilter();
			List<string> all_files = new List<string>();
			List<FileEntry> fileentries = new List<FileEntry>();
			foreach (FileData fd in FindFiles.EnumerateFiles(rootdir, filter))
			{
				FileEntry entry = new FileEntry();
				entry.FilePath = fd.Path;
				entry.FileSize = fd.Size;
				entry.FileTime = fd.LastWriteTimeUtc;
				fileentries.Add(entry);
			}

			FilesHash.Hash(rootdir, fileentries);
			WriteContentInfo(rootdir, output_filepath, fileentries);
		}

		public void Verify(string rootdir, string input_filepath)
		{
			rootdir = rootdir.EnsureEndsWith("\\");

			List<FileEntry> verifiedfileentries = new List<FileEntry>();
			ReadContentInfo(rootdir, input_filepath, verifiedfileentries);

			Dictionary<string, FileEntry> verifiedfileentrydict = new Dictionary<string, FileEntry>();
			foreach (FileEntry fe in verifiedfileentries)
			{
				string relativefilepath = fe.FilePath.Substring(rootdir.Length);
				verifiedfileentrydict.Add(relativefilepath.ToLower(), fe);
			}

			Int64 verifyErrors = 0;
			Int64 verifyWarnings = 0;

			FileFilter filter = new FileFilter();
			List<string> all_files = new List<string>();
			List<FileEntry> fileentries = new List<FileEntry>();
			Dictionary<string, FileEntry> fileentrydict = new Dictionary<string, FileEntry>();
			foreach (FileData fd in FindFiles.EnumerateFiles(rootdir, filter))
			{
				string relativefilepath = fd.Path.Substring(rootdir.Length);
				if (verifiedfileentrydict.ContainsKey(relativefilepath.ToLower()))
				{
					FileEntry fe = new FileEntry();
					fe.FilePath = fd.Path;
					fe.FileSize = fd.Size;
					fe.FileTime = fd.LastWriteTimeUtc;
					fileentries.Add(fe);
					fileentrydict.Add(relativefilepath.ToLower(), fe);
				}
				else
				{
					if (Verbose)
					{
						verifyWarnings++;
						Console2.WriteLineWithColor(ConsoleColor.Yellow, "warning: file '{0}' is not part of the build content ", relativefilepath);
					}
				}
			}

			FilesHash.Hash(rootdir, fileentries);

			foreach (FileEntry vfe in verifiedfileentries)
			{
				string relativefilepath = vfe.FilePath.Substring(rootdir.Length);
				if (!fileentrydict.ContainsKey(relativefilepath.ToLower()))
				{
					verifyErrors++;
					Console2.WriteLineWithColor(ConsoleColor.Red, "error: file '{0}' is missing from the build content ", relativefilepath);
				}
			}

			foreach (FileEntry fe in fileentries)
			{
				string relativefilepath = fe.FilePath.Substring(rootdir.Length);

				FileEntry vfe;
				if (verifiedfileentrydict.TryGetValue(relativefilepath.ToLower(), out vfe))
				{
					// Compare hashes
					if (vfe.CompareContent(fe) != 0)
					{
						verifyErrors++;
						Console2.WriteLineWithColor(ConsoleColor.Red, "error: file '{0}' content is not the same as original", relativefilepath);
					}
				}
				else
				{
					verifyWarnings++;
					if (Verbose)
					{
						Console2.WriteLineWithColor(ConsoleColor.Yellow, "warning: file '{0}' is not part of the build content ", relativefilepath);
					}
				}
			}

			if (verifyErrors > 0)
			{
				Console2.WriteLineWithColor(ConsoleColor.Red, "error: content has detected changes");
			}
			else
			{
				if (verifyWarnings > 0)
				{
					if (Verbose)
					{
						Console2.WriteLineWithColor(ConsoleColor.Yellow, "warning: some additional files detected");
					}
				}
				Console2.WriteLineWithColor(ConsoleColor.Green, "info: content verified, everything OK!");
			}

		}


		public void WriteContentInfo(string rootdir, string filepath, List<FileEntry> entries)
		{
			FileStream writestream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
			StreamWriter writer = new StreamWriter(writestream);
			foreach (var fe in entries)
			{
				string relativefilepath = fe.FilePath.Substring(rootdir.Length);
				writer.WriteLine("Path={0}", relativefilepath);
				writer.WriteLine("Size={0}", fe.FileSize);
				writer.WriteLine("Time={0}", fe.FileTime.ToUniversalTime().Ticks);
				writer.WriteLine("Hash=[");
				foreach (var hash in fe.ChunkHashes)
				{
					writer.WriteLine("  " + hash.ToString());
				}
				writer.WriteLine("]");
			}

			writer.Flush();
			writer.Close();
			writestream.Close();
		}

		public void ReadContentInfo(string rootdir, string filepath, List<FileEntry> entries)
		{
			FileStream readstream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None);
			StreamReader reader= new StreamReader(readstream);
			FileEntry fe = null;
			while (reader.EndOfStream==false)
			{
				string line = reader.ReadLine();
				if (line.StartsWith("Path="))
				{
					fe = new FileEntry();
					fe.FilePath = line.Substring("Path=".Length);
					fe.FilePath = Path.Combine(rootdir, fe.FilePath);
				}
				else if (line.StartsWith("Size="))
				{
					string sizestr = line.Substring("Size=".Length);
					Int64 size = Int64.Parse(sizestr);
					fe.FileSize = size;
				}
				else if (line.StartsWith("Time="))
				{
					string sizestr = line.Substring("Time=".Length);
					Int64 ticks = Int64.Parse(sizestr);
					fe.FileTime = new DateTime(ticks, DateTimeKind.Utc);
				}
				else if (line.StartsWith("Hash=["))
				{

				}
				else if (line.StartsWith("  "))
				{
					string hashstr = line.Substring("  ".Length);
					byte[] hashbytes = Utils.ToHashBytes(hashstr);
					Hash256 hash = Hash256.ConstructTake(hashbytes);
					fe.ChunkHashes.Add(hash);
				}
				else if (line == "]")
				{
					if (fe != null)
					{
						entries.Add(fe);
						fe = null;
					}
				}
			}

			reader.Close();
			reader.Close();
		}
	}
}
