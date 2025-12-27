using System;
using System.IO;
using System.Text;

namespace nodesCatchNext;

internal static class Logger
{
	private static readonly object _lock = new object();

	private static readonly string LogFilePath = Path.Combine(Utils.StartupPath(), "logs", "app.log");

	private static bool _initialized = false;

	private static void EnsureInitialized()
	{
		if (_initialized)
		{
			return;
		}
		lock (_lock)
		{
			if (_initialized)
			{
				return;
			}
			try
			{
				string directoryName = Path.GetDirectoryName(LogFilePath);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				_initialized = true;
			}
			catch
			{
			}
		}
	}

	public static void Debug(string message)
	{
		WriteLog("DEBUG", message);
	}

	public static void Warn(string message)
	{
		WriteLog("WARN", message);
	}

	public static void Error(string message, Exception ex = null)
	{
		if (ex != null)
		{
			WriteLog("ERROR", message + ": " + ex.Message + "\n" + ex.StackTrace);
		}
		else
		{
			WriteLog("ERROR", message);
		}
	}

	private static void WriteLog(string level, string message)
	{
		try
		{
			EnsureInitialized();
			if (!_initialized)
			{
				return;
			}
			string text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
			lock (_lock)
			{
				File.AppendAllText(LogFilePath, text + Environment.NewLine, Encoding.UTF8);
			}
		}
		catch
		{
		}
	}
}
