using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ColoredConsole
{
	public static class Console2
	{
		private static List<ILog> Loggers;

		static Console2()
		{
			Loggers = new List<ILog>();
			OnlyCollectLogger();
			OriginalForegroundColor = Console.ForegroundColor;
		}

		public static ConsoleColor OriginalForegroundColor { get; set; }

		public static void OnlyCollectLogger()
		{
			Loggers.Clear();
			ILog logger = new CollectorLog();
			logger.Open();
			Loggers.Add(logger);
		}

		public static void OnlyFileLogger(string name)
		{
			Loggers.Clear();
			ILog logger = new FileLogger(name);
			logger.Open();
			Loggers.Add(logger);
		}

		public static void Close()
		{
			foreach (ILog log in Loggers)
				log.Close();

			/// Restore anyway, no matter what happened
			Console.ForegroundColor = OriginalForegroundColor;
		}

		public static void DumpLog(ILog logger)
		{ 
			foreach(ILog log in Loggers)
			{
				ILogWriter writer = log as ILogWriter;
				if (writer != null)
					writer.WriteTo(logger);
			}
		}

		public static void Write(string line)
		{
			Console.Write(line);
			foreach(ILog logger in Loggers) logger.Log(line);
		}

		public static void Write(string _format, params object[] _vars)
		{
			string line;
			if (_vars == null || _vars.Length == 0)
				line = _format;
			else
				line = String.Format(_format, _vars);

			Write(line);
		}

		public static void WriteLine(string _text)
		{
			Console.WriteLine(_text);
			foreach (ILog logger in Loggers) logger.LogLine(_text);
		}

		public static void WriteLine(string _format, params object[] _vars)
		{
			string line;
			if (_vars == null || _vars.Length == 0)
				line = _format;
			else
				line = String.Format(_format, _vars);

			WriteLine(line);
		}

		public static void WriteWithColor(ConsoleColor _color, string _format, params object[] _vars)
		{
			ConsoleColor oldcolor = Console.ForegroundColor;
			Console.ForegroundColor = _color;
			string line = String.Format(_format, _vars);
			Console2.Write(line);
			Console.ForegroundColor = oldcolor;
		}

		public static void WriteSuccesLine(string _format, params object[] _vars)
		{
			ConsoleColor oldcolor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			string line = String.Format(_format, _vars);
			Console2.WriteLine(line);
			Console.ForegroundColor = oldcolor;
		}

		public static void WriteErrorLine(string _format, params object[] _vars)
		{
			ConsoleColor oldcolor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			string line = String.Format(_format, _vars);
			Console2.WriteLine(line);
			Console.ForegroundColor = oldcolor;
		}

		public static void WriteWarningLine(string _format, params object[] _vars)
		{
			ConsoleColor oldcolor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			string line = String.Format(_format, _vars);
			Console2.WriteLine(line);
			Console.ForegroundColor = oldcolor;
		}

		public static void WriteLineWithColor(ConsoleColor _color, string _format, params object[] _vars)
		{
			ConsoleColor oldcolor = Console.ForegroundColor;
			Console.ForegroundColor = _color;
			string line = String.Format(_format, _vars);
			Console2.WriteLine(line);
			Console.ForegroundColor = oldcolor;
		}
	}


	public interface ILog
	{
		bool Open();
		void Close();

		void Log(string l);
		void LogLine(string l);
	}

	public interface ILogWriter
	{
		void WriteTo(ILog logger);
	}

	public class CollectorLog : ILog, ILogWriter
	{
		private string line_ = string.Empty;

		private List<string> log_ = new List<string>();
		
		public bool Open()
		{
			log_ = new List<string>();
			return true; 
		}
		
		public void Close()
		{
			log_.Clear();
		}
		
		public void Log(string l)
		{
			line_ = line_ + l;
		}

		public void LogLine(string l)
		{
			if (!String.IsNullOrEmpty(line_))
			{
				Log(l);
				log_.Add(line_);
				line_ = string.Empty;
			}
			else
			{
				log_.Add(l);
			}
		}

		public void WriteTo(ILog logger)
		{
			foreach (string line in log_)
				logger.LogLine(line);
		}
	}

	public class StreamWriterLog : ILog
	{
		public StreamWriterLog(string indent, StreamWriter writer)
		{
			Indent = indent;
			Writer = writer;
		}
		private string Indent { get; set; }
		private StreamWriter Writer { get; set; }

		public bool Open()
		{ return true; }
		public void Close()
		{ }

		public void Log(string l)
		{
		}
		public void LogLine(string l)
		{
			Writer.Write(Indent);
			Writer.WriteLine(l);
		}
	}

	public class FileLogger : ILog
	{
		private string LogFilename = string.Empty;
		private FileStream LogFileStream = null;
		private StreamWriter LogFileWriter = null;
		public FileLogger(string _filename)
		{
			LogFilename = _filename;
		}

		public bool Open()
		{
			int index = 0;
			int retry = 10;
			string logfilename = String.Format("{0}.log", LogFilename);
			while (retry > 0)
			{
				try
				{
					LogFileStream = new FileStream(logfilename, FileMode.Append, FileAccess.Write, FileShare.None);
					LogFileWriter = new StreamWriter(LogFileStream);
					return true;
				}
				catch (Exception)
				{
					logfilename = String.Format("{0}.{1:X4}.log", LogFilename, index);
					++index;
					--retry;
				}
			}
			return false;
		}

		public void Close()
		{
			LogFileWriter.Close();
			LogFileStream.Close();
		}

		public void Log(string line)
		{
			LogFileWriter.Write(line);
		}

		public void LogLine(string line)
		{
			LogFileWriter.WriteLine(line);
		}
	}

}
