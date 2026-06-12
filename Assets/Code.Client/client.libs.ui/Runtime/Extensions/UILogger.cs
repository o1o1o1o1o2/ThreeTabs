using System;

// ReSharper disable once CheckNamespace
internal class UILogger
{
    private static readonly LogCat Logger = new("UI", LogFormatter.Color.yellow);

    public static void LogInfo(string message, params object[] args) => Logger.LogInfo(message, args);
    public static void LogWarning(string message, params object[] args) => Logger.LogWarning(message, args);
    public static void LogError(string message, params object[] args) => Logger.LogError(message, args);
    public static void LogException(Exception ex) => Logger.LogException(ex);
}