using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiskLogger : MonoBehaviour
{
	private StreamWriter writer;

	private const string dir = "Logs\\";

	private const int MaxLogFileCount = 25;

	private void Awake()
	{
		int num;
		if (!Directory.Exists("Logs\\"))
		{
			Directory.CreateDirectory("Logs\\");
		}
		else
		{
			string[] files = Directory.GetFiles("Logs\\", "*.log");
			if (files.Length > 25)
			{
				num = 0;
				foreach (string item in files.OrderBy((string f) => File.GetCreationTime(f)))
				{
					try
					{
						File.Delete(item);
						num++;
						if (files.Length - num <= 25)
						{
							goto IL_009b;
						}
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
				}
				goto IL_009b;
			}
		}
		goto IL_00b0;
		IL_009b:
		Debug.Log($"{num} old log files deleted");
		goto IL_00b0;
		IL_00b0:
		writer = new StreamWriter(GetPath());
		Debug.Log("Timestamp: " + DateTime.UtcNow.ToString());
		Debug.Log("Current version: 1.27.5");
		Application.logMessageReceived += LogMessageReceived;
	}

	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		writer.Write(' ');
		writer.Write(GetPrefix(type));
		writer.Write(' ');
		writer.Write(condition);
		if (type != LogType.Log && !string.IsNullOrWhiteSpace(stackTrace))
		{
			writer.WriteLine();
			writer.Write('\t');
			writer.WriteLine(stackTrace);
		}
		writer.WriteLine();
	}

	private static string GetPrefix(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
			return "[ERROR]";
		case LogType.Assert:
			return "[ASSERT]";
		case LogType.Warning:
			return "[WARNING]";
		case LogType.Log:
			return "[LOG]";
		case LogType.Exception:
			return "[EXCEPTION]";
		default:
			return "[UNKNOWN]";
		}
	}

	private void OnDestroy()
	{
		writer.Dispose();
		Application.logMessageReceived -= LogMessageReceived;
	}

	private string GetPath()
	{
		return "Logs\\" + DateTimeOffset.Now.ToUnixTimeSeconds() + "-" + SceneManager.GetActiveScene().name + ".log";
	}
}
