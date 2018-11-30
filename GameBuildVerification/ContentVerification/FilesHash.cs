using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using ColoredConsole;

namespace ContentVerification
{
	class FilesHash
	{
		/// C# multi-core hashing using Actor Model
		public struct Result
		{
			public int ExitCode { get; set; }
			public int NumFiles { get; set; }
			public int NumChunks { get; set; }
			public Int64 DataSize { get; set; }
			public Int64 Throughput { get; set; }
			public TimeSpan Duration { get; set; }
			public List<string> Errors { get; set; }
		}

		public static Result Hash(string RootDir, List<FileEntry> _meta)
		{
			Queue<FileToHash> files_hashed = new Queue<FileToHash>();
			Queue<FileToHash> files_to_hash = new Queue<FileToHash>();
			foreach (FileEntry m in _meta)
			{
				m.ChunkHashes.Clear();
				if (m.FileSize > 0)
				{
					FileToHash f = new FileToHash();
					f.Entry = m;

					int num_chunks;
					FileChunks.ComputeNumberOfChunks(f.Entry.FileSize, out num_chunks);
					f.ChunkHashes = new List<Hash256>();
					for (int i = 0; i < num_chunks; ++i)
						f.ChunkHashes.Add(Hash256.NULL_HASH);

					files_to_hash.Enqueue(f);
					files_hashed.Enqueue(f);
				}
				else
				{
					m.ChunkHashes.Add(Hash256.NULL_HASH);
				}
			}

			int num_cpu = Environment.ProcessorCount;
			int max_workers = num_cpu * 8;
			ReadActor reader = new ReadActor(files_to_hash);
			List<IFlowActor> workers = new List<IFlowActor>();
			for (int i = 0; i < max_workers; ++i)
				workers.Add(new WorkItem(reader));

			Stopwatch timer = new Stopwatch();
			timer.Start();
			if (files_to_hash.Count > 0)
			{
				Flow.Engine engine = new Flow.Engine(Environment.ProcessorCount);
				engine.Execute(workers);
			}
			timer.Stop();

			/// Make sure we never divide by zero
			long elapsed_ticks = timer.ElapsedTicks + 1;

			Result r = default(Result);
			r.Duration = timer.Elapsed;
			r.NumFiles = reader.NumFiles;
			foreach (IFlowActor actor in workers)
			{
				WorkItem item = actor as WorkItem;
				r.NumChunks += item.TotalChunks;
			}
			r.DataSize = reader.DataSize;
			r.Throughput = (Int64)((double)reader.DataSize / ((double)elapsed_ticks / (double)Stopwatch.Frequency));
			r.ExitCode = (reader.Errors.Count == 0) ? 0 : -1;
			r.Errors = reader.Errors;

			Console2.WriteLineWithColor(ConsoleColor.Green, "info: hashed {0} files, {1} chunks, {2} of data, throughput {3}/s, duration {4}", r.NumFiles, r.NumChunks, r.DataSize.ToByteSize(), r.Throughput.ToByteSize(), r.Duration.ToPerf());

			/// Commit all processed files to their meta information
			foreach (FileToHash f in files_hashed)
			{
				var m = f.Entry;
				m.ChunkHashes.Clear();
				foreach (Hash256 h in f.ChunkHashes)
				{
					m.ChunkHashes.Add(h);
				}
			}

			return r;
		}

		public class FileToHash
		{
			public FileEntry Entry { get; set; }
			public Int64 FileSize { get; set; }
			public List<Hash256> ChunkHashes { get; set; }
		}

		public class FileChunk
		{
			public FileToHash Owner { get; set; }
			public Flow.Buffer ReadBuffer { get; set; }

			public int ChunkIndex { get; set; }
			public int ChunkLength { get; set; }

			public bool IsEmpty { get { return Owner == null; } }

			public FileChunk()
			{
				ReadBuffer = new Flow.Buffer(new byte[FileChunks.DefaultChunkSize], 0);
			}

			public void Reset()
			{
				Owner = null;
				ReadBuffer.Reset();
				ChunkIndex = -1;
				ChunkLength = 0;
			}
		}

		public class ReadActor
		{
			private Queue<FileToHash> files_to_hash_;

			private FileToHash current_file_;
			private FileStream current_file_stream_;
			private int current_chunk_index;
			private Stopwatch timer_;

			public ReadActor(Queue<FileToHash> _files_to_hash)
			{
				files_to_hash_ = _files_to_hash; 

				current_file_ = null;
				current_file_stream_ = null;
				current_chunk_index = 0;
				timer_ = new Stopwatch();
				timer_.Start();

				TotalFiles = _files_to_hash.Count;
				CurrentFile = 0;
				Errors = new List<string>();

				NumFiles = 0;
				NumChunks = 0;
				DataSize = 0;
				ReadTime = 0.001;
			}

			public int TotalFiles { get; set; }
			public int CurrentFile { get; set; }
			public List<string> Errors { get; set; }

			public int NumFiles { get; set; }
			public int NumChunks { get; set; }
			public Int64 DataSize { get; set; }
			public double ReadTime { get; set; }

			public bool Read(FileChunk chunk)
			{
				while (current_file_ == null && files_to_hash_.Count > 0)
				{
					CurrentFile += 1;

					current_file_ = files_to_hash_.Peek();
					files_to_hash_.Dequeue();

					try
					{
						Console2.Write("info: processing ({0}/{1}) \"{2}\"", CurrentFile, TotalFiles, current_file_.Entry.FilePath);

						current_file_stream_ = new FileStream(current_file_.Entry.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
						current_chunk_index = 0;
						current_file_.FileSize = current_file_stream_.Length;
						NumFiles += 1;

						Console2.Write(" ({0})", current_file_.FileSize.ToByteSize());

						/// Make sure we are going to work with a correctly sized hash array for this file
						int num_chunks;
						FileChunks.ComputeNumberOfChunks(current_file_.FileSize, out num_chunks);
						while (num_chunks < current_file_.ChunkHashes.Count)
							current_file_.ChunkHashes.RemoveAt(num_chunks);
						while (num_chunks > current_file_.ChunkHashes.Count)
							current_file_.ChunkHashes.Add(Hash256.NULL_HASH);

						Console2.WriteLineWithColor(ConsoleColor.Green, " (ok)");
					}
					catch (Exception)
					{
						Console2.WriteLineWithColor(ConsoleColor.Red, " (error)");

						Errors.Add(String.Format("error: processing ({0}/{1}) \"{2}\" ({3}) (reason: {4})", CurrentFile, TotalFiles, current_file_.Entry.FilePath, current_file_.Entry.FileSize.ToByteSize(), "failed to open"));
						current_file_ = null;
						current_file_stream_ = null;
					}
				}


				if (current_file_ != null)
				{
					chunk.ChunkIndex = current_chunk_index++;
					chunk.Owner = current_file_;

					timer_.Reset();
					timer_.Start();

					Int64 out_chunk_offset;
					int out_chunk_length;
					FileChunks.ComputeLengthAndOffset(chunk.ChunkIndex, chunk.Owner.FileSize, out out_chunk_offset, out out_chunk_length);
					current_file_stream_.Read(chunk.ReadBuffer.Data, 0, out_chunk_length);
					chunk.ChunkLength = out_chunk_length;
					chunk.ReadBuffer.Size = chunk.ChunkLength;

					timer_.Stop();

					/// Keep track of some performance measurements
					NumChunks += 1;
					DataSize += out_chunk_length;
					ReadTime += timer_.ElapsedTicks;

					/// Was this the last chunk of the file, 
					if (current_chunk_index == chunk.Owner.ChunkHashes.Count)
					{
						current_file_stream_.Close();
						current_file_stream_ = null;
						current_file_ = null;
					}

					return true;
				}

				return false;
			}

		}

		public class WorkItem : IFlowActor
		{
			private enum EState
			{
				READ,
				HASH,
				DONE,
			}

			private EState state_;
			private ReadActor reader_;
			private FileChunk chunk_;
			private Skein256 hasher_;

			public WorkItem(ReadActor reader)
			{
				state_ = EState.READ;
				reader_ = reader;
				chunk_ = new FileChunk();
				hasher_ = new Skein256();
				hasher_.Initialize();
				Transition = EFlowState.READ;
				TotalChunks = 0;
				TotalDataSize = 0;
			}

			public int TotalChunks{ get; set; }
			public int TotalDataSize { get; set; }

			public EFlowState Transition { get; set; }

			public void Execute(EFlowState pipe)
			{
				switch (state_)
				{
					case EState.READ:
						{
							if (reader_.Read(chunk_))
							{
								state_ = EState.HASH;
								Transition = EFlowState.WORK;
							}
							else
							{
								state_ = EState.DONE;
								Transition = EFlowState.END;
							}
						} break;
					case EState.HASH:
						{
							TotalChunks += 1;
							TotalDataSize += chunk_.ReadBuffer.Size;
							Hash2(chunk_.ReadBuffer.Data, chunk_.ReadBuffer.Size, chunk_.Owner.ChunkHashes[chunk_.ChunkIndex]);
							chunk_.ReadBuffer.Reset();
							state_ = EState.READ;
							Transition = EFlowState.READ;
						} break;
					default:
						break;
				}
			}

			private void Hash(byte[] data, int length, Hash256 hash)
			{
				byte[] hashbytes = hasher_.ComputeHash(data, 0, length);
				hash.CopyFrom(hashbytes, 0);
			}

			private void Hash2(byte[] data, int length, Hash256 hash)
			{
				ContentVerification.CCore.SkeinHash(data, length, hash.Data);
			}
		}

	}
}
