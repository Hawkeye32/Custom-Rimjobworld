using System;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace rjw
{
	public static class Logger
	{
		private static readonly LogMessageQueue messageQueueRJW = new LogMessageQueue();
		public static void Message(string text)
		{
			bool DevModeEnabled = RJWSettings.DevMode;
			if (!DevModeEnabled) return;
			UnityEngine.Debug.Log(text);
			messageQueueRJW.Enqueue(new LogMessage(LogMessageType.Message, text, StackTraceUtility.ExtractStackTrace()));
		}
		public static void Warning(string text)
		{
			bool DevModeEnabled = RJWSettings.DevMode;
			if (!DevModeEnabled) return;
			UnityEngine.Debug.Log(text);
			messageQueueRJW.Enqueue(new LogMessage(LogMessageType.Warning, text, StackTraceUtility.ExtractStackTrace()));
		}
		public static void Error(string text)
		{
			bool DevModeEnabled = RJWSettings.DevMode;
			if (!DevModeEnabled) return;
			UnityEngine.Debug.Log(text);
			messageQueueRJW.Enqueue(new LogMessage(LogMessageType.Error, text, StackTraceUtility.ExtractStackTrace()));
		}

		public static TimeSpan Time(Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			action();
			stopwatch.Stop();
			return stopwatch.Elapsed;
		}
	}
}
