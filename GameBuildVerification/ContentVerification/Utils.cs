using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using ColoredConsole;

namespace ContentVerification
{
	public class Counter
	{
		public Counter() { Value = 0; }
		public Counter(int _start) { Value = _start; }

		public int Value { get; set; }
		public int Increment() { Value++; return Value; }
		public int Reset() { int v = Value; Value = 0; return v; }

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public static class DirectoryUtils
	{
		public static void CreateDirectories(List<string> directories)
		{
			// Create the directories first, smartly
			HashSet<int> DirectoryDepths = new HashSet<int>();
			Dictionary<int, HashSet<string>> DirectoriesPerDepth = new Dictionary<int, HashSet<string>>();
			foreach (string path in directories)
			{
				int depth = 0;
				string dir = path;
				while (!String.IsNullOrEmpty(dir))
				{
					++depth;
					int seperator_pos = dir.LastIndexOf(Path.DirectorySeparatorChar);
					if (seperator_pos == -1)
						break;
					dir = dir.Substring(0, seperator_pos);
				}

				HashSet<string> dirs;
				if (!DirectoriesPerDepth.TryGetValue(depth, out dirs))
				{
					dirs = new HashSet<string>();
					DirectoriesPerDepth.Add(depth, dirs);
				}
				dirs.Add(path);
				DirectoryDepths.Add(depth);
			}
			// From the biggest depth, start to create directories
			int max_depth = 0;
			foreach (int depth in DirectoryDepths)
			{
				if (depth > max_depth)
					max_depth = depth;
			}
			HashSet<string> CreatedDirectories = new HashSet<string>();
			int actual_created_directories = 0;
			for (int depth = max_depth; depth >= 0; --depth)
			{
				HashSet<string> dirs;
				if (DirectoriesPerDepth.TryGetValue(depth, out dirs))
				{
					foreach (string path in dirs)
					{
						if (!CreatedDirectories.Contains(path))
						{
							string dir = path;
							Directory.CreateDirectory(dir);
							++actual_created_directories;

							while (!String.IsNullOrEmpty(dir))
							{
								int seperator_pos = dir.LastIndexOf(Path.DirectorySeparatorChar);
								if (seperator_pos == -1)
									break;
								dir = dir.Substring(0, seperator_pos);
								if (!CreatedDirectories.Add(dir))
									break;
							}
						}
					}
				}
			}
		}
	}

	public static class String2
	{
		public static bool IsNotEmpty(this string _str)
		{
			return !String.IsNullOrEmpty(_str);
		}
	}

	public static class MyStringExtension
	{
		public static string EnsureEndsWith(this string _path, string _end)
		{
			if (String.IsNullOrEmpty(_path) || _path.EndsWith(_end))
				return _path;
			return _path + _end;
		}
	}

	public static class MyDictionaryExtensions
	{
		public static bool IsMissingKey<K,V>(this Dictionary<K,V> _d, K _key)
		{
			return _d.ContainsKey(_key) == false;
		}

		public static V ReturnValue<K, V>(this Dictionary<K, V> _d, K _key)
		{
			V value;
			if (_d.TryGetValue(_key, out value))
				return value;
			return default(V);
		}

		public static bool IfExist<K, V>(this Dictionary<K, V> _d, K _key, Action<V> _action)
		{
			V value;
			if (_d.TryGetValue(_key, out value))
			{
				_action(value);
				return true;
			}
			return false;
		}

		public static void ForEach<K,V>(this Dictionary<K, V> _d, Action<K, V> _action)
		{
			foreach (var e in _d)
				_action(e.Key, e.Value);
		}
	}

	public static class MyDirectoryInfoExtension
	{
		public static DirectoryInfo Child(this DirectoryInfo _di, string _subdirname)
		{
			DirectoryInfo child = new DirectoryInfo(Path.Combine(_di.FullName, _subdirname));
			return child;
		}
	}

	public static class MyFileInfoExtension
	{
		public static bool IsArchived(this FileInfo _fi)
		{
			return ((_fi.Attributes & FileAttributes.Archive) == 0);
		}
		public static bool IsToBeArchived(this FileInfo _fi)
		{
			return ((_fi.Attributes & FileAttributes.Archive) == FileAttributes.Archive);
		}
		public static void MarkAsArchived(this FileInfo _fi)
		{
			_fi.Attributes = (_fi.Attributes & ~FileAttributes.Archive);
		}
		public static string GetPathOfFile(this FileInfo _fi)
		{
			return Path.GetDirectoryName(_fi.FullName);
		}
		public static string GetNameOfFile(this FileInfo _fi)
		{
			return Path.GetFileName(_fi.FullName);
		}
		public static bool TryMarkAsArchived(this FileInfo _fi)
		{
			try
			{
				_fi.Attributes = (_fi.Attributes & ~FileAttributes.Archive);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static bool TryRenameFilenameOnly(this FileInfo _fi, string _new_filename)
		{
			try
			{
				if (_fi.Exists)
				{
					string fullfilepath = Path.Combine(Path.GetDirectoryName(_fi.FullName), _new_filename);
					_fi.MoveTo(fullfilepath);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static bool TryDelete(this FileInfo _fi)
		{
			try
			{
				if (_fi.Exists)
					_fi.Delete();	
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static void MarkAsToBeArchived(this FileInfo _fi)
		{
			_fi.Attributes = (_fi.Attributes | FileAttributes.Archive);
		}
		public static void RemoveReadOnly(this FileInfo _fi)
		{
			_fi.Attributes = (_fi.Attributes & ~FileAttributes.ReadOnly);
		}
		public static int CompareTo(this FileInfo _this, FileInfo _other)
		{
			int c = _this.Length.CompareTo(_other.Length);
			if (c != 0) return c;
			c = _this.LastWriteTime.CompareTo(_other.LastWriteTime);
			if (c != 0) return c;
			c = _this.CreationTime.CompareTo(_other.CreationTime);
			if (c != 0) return c;
			return 0;
		}

		public static void SetTime(this FileInfo _this, DateTime _dt)
		{
			_this.CreationTime = _dt;
			_this.LastWriteTime = _dt;
			_this.LastAccessTime = _dt;
		}
		
		public static void SyncFileTime(this FileInfo _this, FileInfo _other)
		{
			_this.CreationTime = _other.CreationTime;
			_this.LastWriteTime = _other.LastWriteTime;
			_this.LastAccessTime = _other.LastAccessTime;
		}

		public static FileInfo ChangeExtension(this FileInfo _this, string _new_extension)
		{
			string filename = _this.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(_this.FullName) + _new_extension;
			FileInfo fi = new FileInfo(filename);
			return fi;
		}
	}

	public static class MyBoolExtension
	{
		public static bool NOT(this bool _value)
		{
			return !_value;
		}
	}

	public static class MyTimeSpanExtension
	{
		public static string ToPerf(this TimeSpan ts)
		{
			if (ts.TotalSeconds < 1.0)
			{
				return String.Format("{0:0.##}ms", ts.TotalMilliseconds);
			}
			else if (ts.TotalMinutes < 1.0)
			{
				return String.Format("{0:0.###}s", ts.TotalSeconds);
			}
			else if (ts.TotalHours < 1.0)
			{
				return String.Format("{0:0.###}m", ts.TotalMinutes);
			}
			else
			{
				return String.Format("{0:0.###}h", ts.TotalHours);
			}
		}
	}

	class PercentagePrinter
	{
		private int CursorLeft { get; set; }
		private int CursorTop { get; set; }
		private Int64 LastPercentage { get; set; }
		private Int64 PercentageStep { get; set; }
		private Int64 MaxValue { get; set; }

		public void Start(string str, Int64 max_value)
		{
			MaxValue = max_value;
			LastPercentage = 0;
			PercentageStep = 10;
			Console.Write(str);
			CursorLeft = Console.CursorLeft;
			CursorTop = Console.CursorTop;
			Print(0);
		}

		public void Stop()
		{
			Print(MaxValue);
			Console2.WriteLine("");
		}

		public void Print(Int64 value)
		{
			Int64 percentage = (100 * value) / MaxValue;
			Console.SetCursorPosition(CursorLeft, CursorTop);
			if (percentage >= LastPercentage)
			{
				Console.Write("{0}%", LastPercentage);
				LastPercentage += PercentageStep;
			}
		}
	}

	public struct FileSize : IFormattable
	{
		private ulong _value;

		private const int DEFAULT_PRECISION = 2;

		private static IList<string> Units = new List<string>() { "bytes", "KB", "MB", "GB", "TB" };

		public FileSize(ulong value)
		{
			_value = value;
		}

		public static explicit operator FileSize(ulong value)
		{
			return new FileSize(value);
		}

		override public string ToString()
		{
			return ToString(null, null);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			int precision;

			if (String.IsNullOrEmpty(format))
				return ToString(DEFAULT_PRECISION);
			else if (int.TryParse(format, out precision))
				return ToString(precision);
			else
				return _value.ToString(format, formatProvider);
		}

		/// <summary>
		/// Formats the FileSize using the given number of decimals.
		/// </summary>
		public string ToString(int precision)
		{
			double pow = Math.Floor((_value > 0 ? Math.Log(_value) : 0) / Math.Log(1024));
			pow = Math.Min(pow, Units.Count - 1);
			double value = (double)_value / Math.Pow(1024, pow);
			return value.ToString(pow == 0 ? "F0" : "F" + precision.ToString()) + " " + Units[(int)pow];
		}
	}

	public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
	{
		public object GetFormat(Type formatType)
		{
			if (formatType == typeof(ICustomFormatter)) return this;
			return null;
		}

		/// <summary>
		/// Usage Examples:
		///		Console2.WriteLine(String.Format(new FileSizeFormatProvider(), "File size: {0:fs}", 100));
		/// </summary>

		private const string fileSizeFormat = "fs";
		private const Decimal OneKiloByte = 1024M;
		private const Decimal OneMegaByte = OneKiloByte * 1024M;
		private const Decimal OneGigaByte = OneMegaByte * 1024M;

		public string Format(string format, object arg, IFormatProvider formatProvider)
		{
			if (format == null || !format.StartsWith(fileSizeFormat))
			{
				return defaultFormat(format, arg, formatProvider);
			}

			if (arg is string)
			{
				return defaultFormat(format, arg, formatProvider);
			}

			Decimal size;

			try
			{
				size = Convert.ToDecimal(arg);
			}
			catch (InvalidCastException)
			{
				return defaultFormat(format, arg, formatProvider);
			}

			string suffix;
			if (size > OneGigaByte)
			{
				size /= OneGigaByte;
				suffix = " GB";
			}
			else if (size > OneMegaByte)
			{
				size /= OneMegaByte;
				suffix = " MB";
			}
			else if (size > OneKiloByte)
			{
				size /= OneKiloByte;
				suffix = " kB";
			}
			else
			{
				suffix = " B";
			}

			string precision = format.Substring(2);
			if (String.IsNullOrEmpty(precision)) precision = "2";
			return String.Format("{0:N" + precision + "}{1}", size, suffix);

		}

		private static string defaultFormat(string format, object arg, IFormatProvider formatProvider)
		{
			IFormattable formattableArg = arg as IFormattable;
			if (formattableArg != null)
			{
				return formattableArg.ToString(format, formatProvider);
			}
			return arg.ToString();
		}

	}

	public static class Utils
	{
		static SHA256 GlobalSHA_;
		static List<char> GlobalChars64_ = new List<char>
			{
				'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z', 
				'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z', 
				'0','1','2','3','4','5','6','7','8','9',
				'_','.'
			};

		static Utils()
		{
			GlobalSHA_ = SHA256.Create();
		}

		public static int Encode_32_6(params char[] _characters)
		{
			int h = 0;
			foreach (char c in _characters)
			{
				int i = GlobalChars64_.IndexOf(c);
				h = (h << 6) | (i & 0x3F);
			}
			return h;
		}
		public static Int64 Encode_64_6(params char[] _characters)
		{
			Int64 h = 0;
			foreach (char c in _characters)
			{
				Int64 i = GlobalChars64_.IndexOf(c);
				h = (h << 6) | (i & 0x3F);
			}
			return h;
		}

		private static Dictionary<char, bool> HexCharacters = new Dictionary<char, bool>() 
		{
			{'0', true},{'1', true},{'2', true},{'3', true},{'4', true},{'5', true},{'6', true},{'7', true},{'8', true},{'9', true},
			{'A', true},{'B', true},{'C', true},{'D', true},{'E', true},{'F', true},
			{'a', true},{'b', true},{'c', true},{'d', true},{'e', true},{'f', true}
		};

		public static bool IsHexString(string _str)
		{
			foreach (char c in _str)
			{
				if (!HexCharacters.ContainsKey(c))
					return false;
			}
			return true;
		}

		public static char ToHexDigit(int b)
		{
			return (char)((b < 10) ? ('0' + b) : ('A' + b - 10));
		}

		public static string ToHashString(byte[] bytes)
		{
			return ToHexString(bytes, 64);
		}

		private static string ToHexString(byte[] bytes, int out_str_len)
		{
			int length = bytes.Length;
			char[] chars = new char[length * 2];
			int index = 0;
			for (int n = 0; n < length; n++)
			{
				int value = bytes[n] & 0xff;
				chars[index++] = ToHexDigit(value / 16);
				chars[index++] = ToHexDigit(value % 16);
			}
			string str = new string(chars);
            while (str.Length < out_str_len)
                str = "0" + str;
            return str;
		}

		private static string NullHash = "0000000000000000000000000000000000000000000000000000000000000000";
		public static bool IsNullHash(string _hash)
		{
			return _hash == NullHash;
		}
		private static string ErrorHash = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
		public static bool IsErrorHash(string _hash)
		{
			return _hash == ErrorHash;
		}

		public static bool IsValidHash(string _hash)
		{
			if (_hash.Length != 64)
				return false;

			foreach (char c in _hash)
			{
				if (!Char.IsLetterOrDigit(c) || Char.IsLower(c))
					return false;
			}
			return true;
		}

		static public string HashOfString(string _str)
		{
			byte[] bytes = ASCIIEncoding.ASCII.GetBytes(_str);
			byte[] hash = GlobalSHA_.ComputeHash(bytes);
			string shash = ToHashString(hash);
			return shash;
		}

		public static byte[] ToHashBytesN(string _hashstr, int _size_in_bytes)
		{
			byte[] hash = new byte[_size_in_bytes];

			int nc = 2 * _size_in_bytes;
			string str = _hashstr;
			while (str.Length < nc)
				str = "0" + str;

			for (int i = 0; i < nc; i += 2)
			{
				int b = 0;
				for (int j = 0; j < 2; ++j)
				{
					char c = str[i + j];
					Debug.Assert(Char.IsLetterOrDigit(c) && !Char.IsLower(c));

					int n = 0;
					if (c >= 'A' && c <= 'F')
						n = (byte)((int)10 + ((int)c - (int)'A'));
					else if (c >= 'a' && c <= 'f')
						n = (byte)((int)10 + ((int)c - (int)'a'));
					else if (c >= '0' && c <= '9')
						n = (byte)((int)0 + ((int)c - (int)'0'));

					Debug.Assert(n >= 0 && n <= 15);
					b = (byte)((b << 4) | n);
				}

				hash[i / 2] = (byte)b;
			}
			return hash;
		}

		public static byte[] ToHashBytes(string _hashstr)
		{
			return ToHashBytesN(_hashstr, 32);
		}

		public static void FillByteArray(byte[] _array, byte _filler)
		{
			for (int i = 0; i < _array.Length; ++i)
				_array[i] = _filler;
		}

		public static Int64 NumBytesInGB(int n)
		{
			return (Int64)n * 1024 * 1024 * 1024;
		}

		public static Int64 NumBytesInMB(int n)
		{
			return (Int64)n * 1024 * 1024;
		}

		public static int Compare(byte[] _left, int _left_start, byte[] _right, int _right_start, int _count)
		{
			for (int j = 0; j < _count; j++)
			{
				byte m = _left[_left_start + j];
				byte o = _right[_right_start + j];
				if (m < o) return -1;
				else if (m > o) return 1;
			}
			return 0;
		}

		public static int Compare(int a, int b)
		{
			if (a < b)
				return -1;
			else if (a > b)
				return 1;
			else
				return 0;
		}

		public static int Compare(Int64 a, Int64 b)
		{
			if (a < b)
				return -1;
			else if (a > b)
				return 1;
			else
				return 0;
		}

		public static string ToByteSize(this Int64 size)
		{
			return String.Format(new FileSizeFormatProvider(), "{0:fs}", size);
		}

		public static int AlignInt32(int _value, int _alignment)
		{
			int aligned = (_value + (_alignment - 1)) & ~(_alignment - 1);
			return aligned;
		}
		public static long AlignInt64(long _value, int _alignment)
		{
			int aligned = (int)(_value & (_alignment - 1));
			if (aligned == 0)
				return _value;
			return _value + (_alignment - aligned);
		}

		public static void Test1()
		{
			Int64 value = 0;
			
			value = 0;
			Debug.Assert(AlignInt64(value, 64) == value);
			value = 1;
			Debug.Assert(AlignInt64(value, 64) == (value + 63));
			value = 64;
			Debug.Assert(AlignInt64(value, 64) == value);
			value = 65;
			Debug.Assert(AlignInt64(value, 64) == (value + 63));
			value = ((Int64)1024 * 1024 * 1024 * 4) + 65;
			Debug.Assert(AlignInt64(value, 64) == (value + 63));

		}

		public static string[] ExplodeList(string list, char split)
		{
			string[] parts = list.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; ++i )
			{
				parts[i] = parts[i].Trim();
			}
			return parts;
		}

		public static string GetFirstFromList(string list, char split)
		{
			string value = string.Empty;
			string[] parts = list.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length > 0)
			{
				value = parts[0].Trim();
			}
			return value;
		}

		public static string AddToList(string list, char split, string add)
		{
			string[] parts = list.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length > 0)
			{
				bool exists = false;
				foreach(string p in parts)
				{
					string entry = p.Trim();
					if (String.Compare(entry, add, true) == 0)
					{
						exists = true;
						break;
					}
				}
				if (!exists)
					list = list + split + add;
			}
			return list;
		}

		public static bool SplitKeyValue(string line, char split, out string key, out string value)
		{
			string[] parts = line.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2)
			{
				key = parts[0].Trim();
				value = parts[1].Trim();
				return true;
			}
			else
			{
				key = string.Empty;
				value = string.Empty;
				return false;
			}
		}

		public static string ShortenString(string text, int head, int tail, string between)
		{
			string shorter = text;
			if (text.Length > (head + tail))
			{
				string headstr = text.Substring(0, head);
				string tailstr = text.Substring(text.Length - tail, tail);
				shorter = headstr + between + tailstr;
			}
			return shorter;
		}

		public static int WriteString2(string str, BinaryWriter writer)
		{
			int l = Encoding.ASCII.GetBytes(str, 0, str.Length, buffer_, 0);
			writer.Write((short)l);
			writer.Write(buffer_, 0, l);
			return 2 + l;
		}

		private static byte[] buffer_ = new byte[8192];
		public static int WriteString4(string str, BinaryWriter writer)
		{
			int l = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer_, 0);
			writer.Write(l);
			writer.Write(buffer_, 0, l);
			return l;
		}

		public static string ReadString2(BinaryReader reader)
		{
			int l = reader.ReadInt16();
			reader.Read(buffer_, 0, l);
			string str = Encoding.UTF8.GetString(buffer_, 0, l);
			return str;
		}

		public static string ReadString4(BinaryReader reader)
		{
			int l = reader.ReadInt32();
			reader.Read(buffer_, 0, l);
			string str = Encoding.UTF8.GetString(buffer_, 0, l);
			return str;
		}

		public static int WriteString(string str, int alignment, BinaryWriter writer)
		{
			int l = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer_, 0);
			writer.Write(l);
			int n = Utils.AlignInt32(l, alignment);
			writer.Write(buffer_, 0, n);
			return n;
		}
	}


	/*
	 * Wildcard
	 *
	 * Wildcard patterns have three metacharacters:
	 *
	 * A ? is equivalent to .
	 * A * is equivalent to .*
	 * A , is equivalent to |
	 *
	 * Note that by each alternative is surrounded by \A...\z to anchor
	 * at the edges of the string.
	 */
	internal class Wildcard
	{
		internal Wildcard(String pattern, bool caseInsensitive)
		{
			_pattern = pattern;
			_caseInsensitive = caseInsensitive;
			_always = pattern == ".";
			_direct_matches = new Dictionary<string, string>();
			_auto_remove_direct_match_when_matched = true;
		}

		internal bool _always;
		internal String _pattern;
		internal bool _caseInsensitive;
		internal Regex _regex;
		internal bool _auto_remove_direct_match_when_matched;
		internal Dictionary<string, string> _direct_matches;

		protected static Regex metaRegex = new Regex("[\\+\\{\\\\\\[\\|\\(\\)\\.\\^\\$]");
		protected static Regex questRegex = new Regex("\\?");
		protected static Regex starRegex = new Regex("\\*");
		protected static Regex commaRegex = new Regex(",");
		protected static Regex slashRegex = new Regex("(?=/)");
		protected static Regex backslashRegex = new Regex("(?=[\\\\:])");

		internal static void UnitTest()
		{
			Wildcard w1 = new Wildcard("path*\\file???.*,path*\\file????.*", true);

			bool b11 = w1.IsMatch("path\\folder\\file123.ext");
			bool b13 = w1.IsMatch("path\\folder\\folder\\file123.ext");
			bool b14 = w1.IsMatch("path\\folder\\folder\\file1234.ext");
			bool b12 = w1.IsMatch("path\\file123.ext");

			Debug.Assert(b11);
			Debug.Assert(b13);
			Debug.Assert(b14);
			Debug.Assert(b12);

			Wildcard w2 = new Wildcard("path\\folder\\file123.ext", true);
			bool b21 = w2.IsMatch("path\\folder\\file123.ext");
			Debug.Assert(b21);
		}

		public void RemoveDirectMatch(string input)
		{
			_direct_matches.Remove(_caseInsensitive ? input : input.ToLower());
		}

		public void ForEachDirectMatch(Action<string> _action)
		{
			foreach(var v in _direct_matches)
				_action(v.Value);
		}

		/*
		 * IsMatch returns true if the input is an exact match for the
		 * wildcard pattern.
		 */
		internal /*public*/ bool IsMatch(string input)
		{
			bool result = _always;
			if (!result)
			{
				EnsureRegex();

				string search = _caseInsensitive ? input.ToLower() : input;
				result = _direct_matches.ContainsKey(search);
				if (!result)
				{
					result = _regex.IsMatch(search);
				}
				else if (_auto_remove_direct_match_when_matched)
				{
					_direct_matches.Remove(search);
				}
			}
			return result;
		}
#if DONT_COMPILE
		internal /*public*/ String Pattern {
			get {
				return _pattern;
			}
		}
#endif
		/*
		 * Builds the matching regex when needed
		 */
		protected void EnsureRegex()
		{
			// threadsafe without protection because of gc

			if (_regex != null)
				return;

			_regex = RegexFromWildcard(_pattern, _caseInsensitive);
		}

		/*
		 * Basic wildcard -> Regex conversion, no slashes
		 */
		protected virtual Regex RegexFromWildcard(String pattern, bool caseInsensitive)
		{
			_direct_matches.Clear();

			string[] patterns = pattern.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			pattern = string.Empty;
			foreach (string p in patterns)
			{
				string direct = p.Trim();

				if (direct.IndexOfAny(new char[] { '?', '*' }, 0) == -1)
				{
					string direct_lower = _caseInsensitive ? direct.ToLower() : direct;
					if (!_direct_matches.ContainsKey(direct_lower))
						_direct_matches.Add(direct_lower, direct);
				}
				else
				{
					if (String.IsNullOrEmpty(pattern))
						pattern = direct;
					else
						pattern = pattern + "," + direct;
				}
			}

			RegexOptions options = RegexOptions.None;

			// match right-to-left (for speed) if the pattern starts with a *

			if (pattern.Length > 0 && pattern[0] == '*')
				options = RegexOptions.RightToLeft | RegexOptions.Singleline;
			else
				options = RegexOptions.Singleline;

			// case insensitivity

			if (caseInsensitive)
				options |= RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

			// Remove regex metacharacters

			pattern = metaRegex.Replace(pattern, "\\$0");

			// Replace wildcard metacharacters with regex codes

			pattern = questRegex.Replace(pattern, ".");
			pattern = starRegex.Replace(pattern, ".*");
			pattern = commaRegex.Replace(pattern, "\\z|\\A");

			// anchor the pattern at beginning and end, and return the regex

			return new Regex("\\A" + pattern + "\\z", options);
		}
	}

	abstract internal class WildcardPath : Wildcard
	{
		internal /*public*/ WildcardPath(String pattern, bool caseInsensitive)
			: base(pattern, caseInsensitive)
		{
		}

		private Regex _suffix;

		/*
		 * IsSuffix returns true if a suffix of the input is an exact
		 * match for the wildcard pattern.
		 */
		internal /*public*/ bool IsSuffix(String input)
		{
			EnsureSuffix();
			return _suffix.IsMatch(input);
		}

		/*
		 * Builds the matching regex when needed
		 */
		protected void EnsureSuffix()
		{
			// threadsafe without protection because of gc

			if (_suffix != null)
				return;

			_suffix = SuffixFromWildcard(_pattern, _caseInsensitive);
		}


		/*
		 * Specialize for forward-slash and backward-slash cases
		 */
		protected abstract Regex SuffixFromWildcard(String pattern, bool caseInsensitive);
		protected abstract Regex[][] DirsFromWildcard(String pattern);
		protected abstract String[] SplitDirs(String input);
	}

	/*
	 * WildcardUrl
	 *
	 * The twist is that * and ? cannot match forward slashes,
	 * and we can do an exact suffix match that starts after
	 * any /, and we can also do a prefix prune.
	 */
	internal class WildcardUrl : WildcardPath
	{
		internal /*public*/ WildcardUrl(String pattern, bool caseInsensitive)
			: base(pattern, caseInsensitive)
		{
		}

		protected override String[] SplitDirs(String input)
		{
			return slashRegex.Split(input);
		}

		protected override Regex RegexFromWildcard(String pattern, bool caseInsensitive)
		{
			RegexOptions options;

			// match right-to-left (for speed) if the pattern starts with a *

			if (pattern.Length > 0 && pattern[0] == '*')
				options = RegexOptions.RightToLeft;
			else
				options = RegexOptions.None;

			// case insensitivity

			if (caseInsensitive)
				options |= RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

			// Remove regex metacharacters

			pattern = metaRegex.Replace(pattern, "\\$0");

			// Replace wildcard metacharacters with regex codes

			pattern = questRegex.Replace(pattern, "[^/]");
			pattern = starRegex.Replace(pattern, "[^/]*");
			pattern = commaRegex.Replace(pattern, "\\z|\\A");

			// anchor the pattern at beginning and end, and return the regex

			return new Regex("\\A" + pattern + "\\z", options);
		}

		protected override Regex SuffixFromWildcard(String pattern, bool caseInsensitive)
		{
			RegexOptions options;

			// match right-to-left (for speed)

			options = RegexOptions.RightToLeft;

			// case insensitivity

			if (caseInsensitive)
				options |= RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

			// Remove regex metacharacters

			pattern = metaRegex.Replace(pattern, "\\$0");

			// Replace wildcard metacharacters with regex codes

			pattern = questRegex.Replace(pattern, "[^/]");
			pattern = starRegex.Replace(pattern, "[^/]*");
			pattern = commaRegex.Replace(pattern, "\\z|(?:\\A|(?<=/))");

			// anchor the pattern at beginning and end, and return the regex

			return new Regex("(?:\\A|(?<=/))" + pattern + "\\z", options);
		}

		protected override Regex[][] DirsFromWildcard(String pattern)
		{
			String[] alts = commaRegex.Split(pattern);
			Regex[][] dirs = new Regex[alts.Length][];

			for (int i = 0; i < alts.Length; i++)
			{
				String[] dirpats = slashRegex.Split(alts[i]);

				Regex[] dirregex = new Regex[dirpats.Length];

				if (alts.Length == 1 && dirpats.Length == 1)
				{
					// common case: no commas, no slashes: dir regex is same as top regex.

					EnsureRegex();
					dirregex[0] = _regex;
				}
				else
				{
					for (int j = 0; j < dirpats.Length; j++)
					{
						dirregex[j] = RegexFromWildcard(dirpats[j], _caseInsensitive);
					}
				}

				dirs[i] = dirregex;
			}

			return dirs;
		}
	}

}


