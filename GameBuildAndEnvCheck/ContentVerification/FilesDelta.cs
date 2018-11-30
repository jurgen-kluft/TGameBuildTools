using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Octodiff;
using Octodiff.Core;
using Octodiff.Diagnostics;
using ColoredConsole;

namespace ContentVerification
{
	public static class FilesDelta
	{
		public static void GlobMergeOldAndNew(string oldpath, string newpath, SortedDictionary<string, FileData> all_files)
		{
			HashSet<string> addedset = new HashSet<string>();

			FileFilter filter = new FileFilter();
			foreach (FileData fd in FindFiles.EnumerateFiles(oldpath, filter))
			{
				string relativepath = fd.Path.Substring(oldpath.Length + 1);
				all_files.Add(relativepath, fd);
				addedset.Add(relativepath.ToLower());
			}

			foreach (FileData fd in FindFiles.EnumerateFiles(newpath, filter))
			{
				string relativepath = fd.Path.Substring(newpath.Length + 1);
				if (!addedset.Contains(relativepath.ToLower()))
				{
					all_files.Add(relativepath, fd);
				}
			}
		}

		public static void Create(string oldpath, string newpath, string deltapath)
		{
			DateTime start = DateTime.Now;

			Console2.WriteLineWithColor(ConsoleColor.Green, "info: Starting delta");

			Int64 totalOldDataSize = 0;
			Int64 totalNewDataSize = 0;
			Int64 totalSignatureDataSize = 0;
			Int64 totalDeltaDataSize = 0;

			FileInfo nonExistingNewFileInfo = new FileInfo(Path.GetTempFileName());
			FileStream nonExistingNewFileStream = nonExistingNewFileInfo.Create();
			nonExistingNewFileStream.Close();

			SortedDictionary<string, FileData> all_files = new SortedDictionary<string, FileData>();
			GlobMergeOldAndNew(oldpath, newpath, all_files);

			foreach(var f in all_files)
			{
				FileData fd = f.Value;

				string relativepath = f.Key;
				string oldfilepath = Path.Combine(oldpath, relativepath);
				string newfilepath = Path.Combine(newpath, relativepath);
				string deltafilepath = Path.Combine(deltapath, relativepath) + ".delta";
				string signaturefilepath = Path.Combine(deltapath, relativepath) + ".signature";

				if (File.Exists(newfilepath))
				{
					// Make sure directories exist in the output (delta folder)
					List<string> dirs_to_create = new List<string>();
					dirs_to_create.Add(Path.GetDirectoryName(signaturefilepath));
					DirectoryUtils.CreateDirectories(dirs_to_create);

					if (File.Exists(oldfilepath))
					{
						// Build signature of oldfile
						var signatureBuilder = new SignatureBuilder();
						using (var basisStream = new FileStream(oldfilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
						using (var signatureStream = new FileStream(signaturefilepath, FileMode.Create, FileAccess.Write, FileShare.Read))
						{
							signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
							totalOldDataSize += basisStream.Length;
							totalSignatureDataSize += signatureStream.Length;
						}

						// Create delta file
						var deltaBuilder = new DeltaBuilder();
						using (var newFileStream = new FileStream(newfilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
						using (var signatureFileStream = new FileStream(signaturefilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
						using (var deltaStream = new FileStream(deltafilepath, FileMode.Create, FileAccess.Write, FileShare.Read))
						{
							totalNewDataSize += newFileStream.Length;
							deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
							totalDeltaDataSize += deltaStream.Length;
						}

						Console2.WriteLineWithColor(ConsoleColor.Green, "info: file '{0}' (old:yes, new:yes), created signature and delta", relativepath);
					}
					else
					{
						// Create delta-file using a non-existing (empty) 'oldfile' since a 'newfile' exists
						nonExistingNewFileStream = nonExistingNewFileInfo.OpenRead();

						// Build signature of oldfile
						var signatureBuilder = new SignatureBuilder();
						using (var basisStream = nonExistingNewFileStream)
						using (var signatureStream = new FileStream(signaturefilepath, FileMode.Create, FileAccess.Write, FileShare.Read))
						{
							signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
							totalOldDataSize += basisStream.Length;
							totalSignatureDataSize += signatureStream.Length;
						}

						// Create delta file
						var deltaBuilder = new DeltaBuilder();
						using (var newFileStream = new FileStream(newfilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
						using (var signatureFileStream = new FileStream(signaturefilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
						using (var deltaStream = new FileStream(deltafilepath, FileMode.Create, FileAccess.Write, FileShare.Read))
						{
							totalNewDataSize += newFileStream.Length;
							deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, null), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
							totalDeltaDataSize += deltaStream.Length;
						}

						Console2.WriteLineWithColor(ConsoleColor.Green, "info: file '{0}' (old:no, new:yes), created signature and delta", relativepath);
					}
				}
				else
				{
					// Create delta-file using a non-existing (empty) 'newfile' since a 'oldfile' exists

					Console2.WriteLineWithColor(ConsoleColor.Green, "info: file '{0}' (old:yes, new:no), skipping signature and delta", relativepath);
				}
			}

			TimeSpan duration = DateTime.Now - start;
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: finished building patch, took {0}", duration.ToPerf());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for old data: {0}", totalOldDataSize.ToByteSize());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for new data: {0}", totalNewDataSize.ToByteSize());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for deltas: {0}", totalDeltaDataSize.ToByteSize());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for signatures: {0}", totalSignatureDataSize.ToByteSize());
		}

		public static void Apply(string oldpath, string deltapath, string newpath)
		{
			DateTime start = DateTime.Now;

			Console2.WriteLineWithColor(ConsoleColor.Green, "info: Applying patch");

			Int64 totalOldDataSize = 0;
			Int64 totalDeltaDataSize = 0;
			Int64 totalNewDataSize = 0;

			FileFilter filter = new FileFilter();
			string fileext = ".delta";
			List<string> all_files = new List<string>();
			foreach (FileData fd in FindFiles.EnumerateFiles(deltapath, filter))
			{
				if (fd.Name.EndsWith(fileext))
				{
					string relativepath = fd.Path.Substring(deltapath.Length + 1);
					relativepath = relativepath.Substring(0, relativepath.Length - fileext.Length);
					all_files.Add(relativepath);
				}
			}

			FileInfo nonExistingNewFileInfo = new FileInfo(Path.GetTempFileName());
			FileStream nonExistingNewFileStream = nonExistingNewFileInfo.Create();
			nonExistingNewFileStream.Close();


			foreach (var f in all_files)
			{
				string relativepath = f;
				string oldfilepath = Path.Combine(oldpath, relativepath);
				string newfilepath = Path.Combine(newpath, relativepath);
				string deltafilepath = Path.Combine(deltapath, relativepath) + ".delta";
				string signaturefilepath = Path.Combine(deltapath, relativepath) + ".signature";

				// Make sure directories exist in the output (delta folder)
				List<string> dirs_to_create = new List<string>();
				dirs_to_create.Add(Path.GetDirectoryName(newfilepath));
				DirectoryUtils.CreateDirectories(dirs_to_create);

				if (File.Exists(oldfilepath))
				{
					// Apply delta file to create new file
					var deltaApplier = new DeltaApplier { SkipHashCheck = false };
					using (var basisStream = new FileStream(oldfilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
					using (var deltaStream = new FileStream(deltafilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
					using (var newFileStream = new FileStream(newfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
					{
						totalOldDataSize += basisStream.Length;
						totalDeltaDataSize += deltaStream.Length;
						deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, null), newFileStream);
						totalNewDataSize += newFileStream.Length;
					}
					Console2.WriteLineWithColor(ConsoleColor.Green, "info: file '{0}' applied patch", relativepath);
				}
				else
				{
					// Apply delta file to create new file
					var deltaApplier = new DeltaApplier { SkipHashCheck = false };
					using (var basisStream = nonExistingNewFileInfo.OpenRead())
					using (var deltaStream = new FileStream(deltafilepath, FileMode.Open, FileAccess.Read, FileShare.Read))
					using (var newFileStream = new FileStream(newfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
					{
						// totalOldDataSize += basisStream.Length;	// basisStream.Length == 0
						totalDeltaDataSize += deltaStream.Length;
						deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, null), newFileStream);
						totalNewDataSize += newFileStream.Length;
					}

					Console2.WriteLineWithColor(ConsoleColor.Yellow, "info: file '{0}' applied patch", relativepath);
				}
			}

			TimeSpan duration = DateTime.Now - start;
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: finished applying patch, took {0}", duration.ToPerf());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for deltas: {0}", totalDeltaDataSize.ToByteSize());
			Console2.WriteLineWithColor(ConsoleColor.Green, "info: total data size for patched data: {0}", totalNewDataSize.ToByteSize());
		}
	}
}
