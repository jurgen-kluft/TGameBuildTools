using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace GameBuildTools
{
	public class FileFilter
	{
		private HashSet<string> included_extensions_ = new HashSet<string>();
		private HashSet<string> ignored_foldernames_ = new HashSet<string>();
		private HashSet<string> ignored_folders_ = new HashSet<string>();
		private HashSet<string> ignored_files_ = new HashSet<string>();
		private HashSet<string> ignored_extensions_ = new HashSet<string>();

		public FileFilter()
		{
		}

		public void AddDirName(string _folder)
		{
			_folder = _folder.TrimEnd('\\');
			ignored_foldernames_.Add(_folder.ToLower());
		}

		public void AddDirPath(string _path)
		{
			_path = _path.ToLower();
			_path = _path.EnsureEndsWith("\\");
			ignored_folders_.Add(_path);
		}

		public void AddFileName(string _filename)
		{
			ignored_files_.Add(_filename.ToLower());
		}

		public void AddFileExtension(string _extension, bool _ignored)
		{
			if (_ignored)
				ignored_extensions_.Add(_extension.ToLower());
			else
				included_extensions_.Add(_extension.ToLower());
		}

		public bool IsFolderNameIgnored(string _dirname)
		{
			return ignored_foldernames_.Contains(_dirname);
		}

		public bool IsPathIgnored(string _path)
		{
			return ignored_folders_.Contains(_path);
		}

		public bool IsFileIgnored(string _filepath)
		{
			string extension = Path.GetExtension(_filepath).ToLower();

			bool is_ignored = (included_extensions_.Count>0 && !included_extensions_.Contains(extension));
			if (!is_ignored)
			{
				if (ignored_extensions_.Contains(extension))
				{
					is_ignored = true;
				}
				else
				{
					string dir = Path.GetDirectoryName(_filepath).ToLower().EnsureEndsWith("\\");
					if (ignored_folders_.Contains(dir))
					{
						is_ignored = true;
					}
					else
					{
						string filename = Path.GetFileName(_filepath).ToLower();
						if (ignored_files_.Contains(filename))
						{
							is_ignored = true;
						}
					}
				}
			}
			return is_ignored;
		}

	}
}
