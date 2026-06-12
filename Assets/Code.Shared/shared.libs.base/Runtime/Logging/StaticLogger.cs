using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
public class StaticLogger {
	private static readonly LogCat Logger = new("Common", LogFormatter.Color.darkgreen);

	[Conditional("DEVELOPMENT_BUILD")]
	public static void LogDev(string message, params object[] args) => Logger.LogDev(message, args);

	public static void LogInfo(string message, params object[] args) => Logger.LogInfo(message, args);
	public static void LogWarning(string message, params object[] args) => Logger.LogWarning(message, args);
	public static void LogError(string message, params object[] args) => Logger.LogError(message, args);
	public static void LogException(Exception ex) => Logger.LogException(ex);
}