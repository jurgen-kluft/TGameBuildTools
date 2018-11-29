using System;
using System.Text;
using System.Collections.Generic;

namespace ContentVerification
{
	public class FileEntry
	{
		public FileEntry()
		{
			ChunkHashes = new List<Hash256>();
		}

		public FileEntry(FileEntry _other)
		{
			ChunkHashes = new List<Hash256>();

			FilePath = _other.FilePath;
			FileSize = _other.FileSize;
			FileTime = _other.FileTime;
		}

		public string FilePath { get; set; }
		public Int64 FileSize { get; set; }
		public DateTime FileTime { get; set; }
		public List<Hash256> ChunkHashes { get; set; }
		public bool HasContent { get { return ChunkHashes.Count > 0; } }

		public Hash256 GetUUID()
		{
			byte[] filepath_bytes = Encoding.Unicode.GetBytes(FilePath.ToLower());
			Skein256 hasher = new Skein256();
			byte[] uuid_bytes = hasher.ComputeHash(filepath_bytes);
			return Hash256.ConstructTake(uuid_bytes);
		}

		public void UpdateFrom(FileEntry _meta)
		{
			FilePath = _meta.FilePath;
			FileSize = _meta.FileSize;
			FileTime = _meta.FileTime;

			ChunkHashes.Clear();
			foreach (Hash256 h in _meta.ChunkHashes)
				ChunkHashes.Add(h);
		}


		public int CompareContent(FileEntry other)
		{
			if (FileSize < other.FileSize)
				return -1;
			if (FileSize > other.FileSize)
				return 1;

			if (ChunkHashes.Count == other.ChunkHashes.Count)
			{
				int c = 0;
				for (int i = 0; i < ChunkHashes.Count && c == 0; i++)
				{
					Hash256 chl = ChunkHashes[i];
					Hash256 chr = other.ChunkHashes[i];
					c = chl.Compare(chr);
					if (c != 0)
						break;
				}
				return c;
			}
			else if (ChunkHashes.Count < other.ChunkHashes.Count)
				return -1;
			else // if (ChunkHashes.Count > other.ChunkHashes.Count)
				return 1;
		}

		public int Compare(FileEntry other)
		{
			int c = String.Compare(FilePath, other.FilePath, true);
			if (c != 0)
				return 1;
			if ((c = FileSize.CompareTo(other.FileSize)) != 0)
				return 1;
			if ((c = FileTime.CompareTo(other.FileTime)) != 0)
				return c;
			return CompareContent(other);
		}

	}
}
