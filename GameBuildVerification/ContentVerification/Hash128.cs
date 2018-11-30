using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ContentVerification
{
	public class Hash128
	{
		public static readonly byte[] hash_null_ = new byte[16] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
		public static readonly byte[] hash_error_ = new byte[16] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
		
		public static Hash128 NULL_HASH { get { return new Hash128(hash_null_, 0); } }
		public static Hash128 ERROR_HASH { get { return new Hash128(hash_error_, 0); } }

		public bool IsErrorHash()
		{
			bool equal = (hash_[0] == hash_error_[0]);
			for (int j = 1; j < Size && equal; j++)
				equal = (hash_[j] == hash_error_[j]);
			return equal;
		}

		public bool IsNullHash()
		{
			bool equal = (hash_[0] == hash_null_[0]);
			for (int j = 1; j < Size && equal; j++)
				equal = (hash_[j] == hash_null_[j]);
			return equal;
		}

		private byte[] hash_;
		private Hash128(byte[] _hash)
		{
			hash_ = _hash;
		}

		public Hash128()
		{
			hash_ = new byte[Size];
			CopyFrom(hash_null_, 0);
		}

		public Hash128(Hash128 _other)
		{
			hash_ = new byte[Size];
			CopyFrom(_other.Data, 0);
		}

		private Hash128(byte[] _array, int _start)
		{
			hash_ = new byte[Size];
			CopyFrom(_array, _start);
		}

		public static readonly int Size = 16;

		public static Hash128 ConstructTake(byte[] _hash)
		{
			return new Hash128(_hash);
		}
		public static Hash128 ConstructCopy(byte[] _hash)
		{
			return new Hash128(_hash, 0);
		}
		public static Hash128 ConstructCopy(byte[] _hash, int start)
		{
			return new Hash128(_hash, start);
		}

		public byte[] Data { get { return hash_; } }

		public byte[] Release()
		{
			byte[] h = hash_;
			hash_ = new byte[Size];
			CopyFrom(hash_null_, 0);
			return h;
		}
		public override int GetHashCode()
		{
			Int32 hashcode = BitConverter.ToInt32(hash_, Size - 4);
			return hashcode; 
		}

		public override string ToString()
		{
			return Utils.ToHashString(hash_);
		}

		public int Copy(Hash128 other)
		{
			return CopyFrom(other.Data, 0);
		}

		public int CopyFrom(byte[] _hash, int _offset)
		{
			for (int j = 0; j < Size; j++)
				hash_[j] = _hash[_offset + j];
			return Size;
		}

		public int CopyTo(byte[] _header)
		{
			return CopyTo(_header, 0);
		}

		public int CopyTo(byte[] _header, int _index)
		{
			for (int j = 0; j < Size; j++)
				_header[j + _index] = hash_[j];
			return Size;
		}

		public void WriteTo(BinaryWriter _writer)
		{
			_writer.Write(hash_);
		}

		public static bool operator ==(Hash128 b1, Hash128 b2)
		{
			bool equal = (b1.hash_[0] == b2.hash_[0]);
			for (int j = 1; j < Size && equal; j++)
				equal = (b1.hash_[j] == b2.hash_[j]);
			return equal;
		}
		public static bool operator !=(Hash128 b1, Hash128 b2)
		{
			bool equal = (b1.hash_[0] == b2.hash_[0]);
			for (int j = 1; j < Size && equal; j++)
				equal = (b1.hash_[j] == b2.hash_[j]);
			return equal == false;
		}

		public override bool Equals(object obj)
		{
			Hash128 h = (Hash128)obj;
			return Compare(h) == 0;
		}

		public int Compare(Hash128 _other)
		{
			return Compare(_other.Data, 0);
		}

		public int Compare(byte[] _other, int _start)
		{
			for (int j = 0; j < Size; j++)
			{
				byte m = hash_[j];
				byte o = _other[_start + j];
				if (m < o) return -1;
				else if (m > o) return 1;
			}
			return 0;
		}

		public static void UnitTest()
		{
			Hash128 h1 = new Hash128();
			Hash128 h2 = new Hash128();

			Hash128 h3 = new Hash128(Hash128.ERROR_HASH);
			Hash128 h4 = new Hash128(Hash128.ERROR_HASH);

			Debug.Assert(h1 == Hash128.NULL_HASH);
			Debug.Assert(h2 == Hash128.NULL_HASH);
			Debug.Assert(h1 != Hash128.ERROR_HASH);
			Debug.Assert(h2 != Hash128.ERROR_HASH);
			Debug.Assert(h1 == h2);
			Debug.Assert(h1.Compare(h2) == 0);
			Debug.Assert(h1.GetHashCode() == h2.GetHashCode());

			Debug.Assert(h3 != Hash128.NULL_HASH);
			Debug.Assert(h4 != Hash128.NULL_HASH);
			Debug.Assert(h3 == Hash128.ERROR_HASH);
			Debug.Assert(h4 == Hash128.ERROR_HASH);
			Debug.Assert(h3 == h4);
			Debug.Assert(h3.Compare(h4) == 0);
			Debug.Assert(h3.GetHashCode() == h4.GetHashCode());

			Debug.Assert(h1 != h3);
			Debug.Assert(h2 != h4);

			Debug.Assert(Hash128.ERROR_HASH != Hash128.NULL_HASH);
			Debug.Assert(Hash128.ERROR_HASH.Compare(Hash128.NULL_HASH) == 1);
			Debug.Assert(Hash128.NULL_HASH.Compare(Hash128.ERROR_HASH) == -1);
			Debug.Assert(Hash128.ERROR_HASH.GetHashCode() != Hash128.NULL_HASH.GetHashCode());
		}
	};
}
