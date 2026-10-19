using System.Diagnostics;
using UnityEngine;

namespace ImmersiveVrToolsCommon.Runtime.Logging
{
	public static class LoggerScoped
	{
		private const string DebugColor = "#969595";

		public static string LogPrefix = "<NeedsSetting>: ";

		[Conditional("ImmersiveVrTools_DebugEnabled")]
		public static void LogDebug(string message)
		{
			UnityEngine.Debug.Log("<color=#969595>" + LogPrefix + message + "</color>");
		}

		[Conditional("ImmersiveVrTools_DebugEnabled")]
		public static void LogDebug(string message, Object context)
		{
			UnityEngine.Debug.Log("<color=#969595>" + LogPrefix + message + "</color>");
		}

		private static void LogInternal(LogType logType, object message, Object context)
		{
			UnityEngine.Debug.unityLogger.Log(logType, string.Empty, LogPrefix + message, context);
		}

		private static void LogInternal(LogType logType, object message)
		{
			UnityEngine.Debug.unityLogger.Log(logType, LogPrefix + message);
		}

		public static void Log(object message)
		{
			LogInternal(LogType.Log, message);
		}

		public static void Log(object message, Object context)
		{
			LogInternal(LogType.Log, message, context);
		}

		public static void LogError(object message)
		{
			LogInternal(LogType.Error, message);
		}

		public static void LogError(object message, Object context)
		{
			LogInternal(LogType.Error, message, context);
		}

		public static void LogWarning(object message)
		{
			LogInternal(LogType.Warning, message);
		}

		public static void LogWarning(object message, Object context)
		{
			LogInternal(LogType.Warning, message, context);
		}
	}
}
