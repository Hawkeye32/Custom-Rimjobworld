using Verse;

namespace rjw
{
	public static class ModLog
	{
		/// <summary>
		/// Logs the given message with [SaveStorage.ModId] appended.
		/// </summary>
		public static void Error(string message)
		{
			Log.Error($"[{SaveStorage.ModId}] {message}");
		}

		/// <summary>
		/// Logs the given message with [SaveStorage.ModId] appended.
		/// </summary>
		public static void Message(string message)
		{
			Log.Message($"[{SaveStorage.ModId}] {message}");
		}

		/// <summary>
		/// Logs the given message with [SaveStorage.ModId] appended.
		/// </summary>
		public static void Warning(string message)
		{
			Log.Warning($"[{SaveStorage.ModId}] {message}");
		}
	}
}
