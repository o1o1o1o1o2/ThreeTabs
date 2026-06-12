using System;

// ReSharper disable once CheckNamespace
public interface ILogger {
	void LogTrace(string message, string tag, LogFormatter.Color color, params object[] args);

	void LogDev(string message, string tag, LogFormatter.Color color, params object[] args);

	void LogInfo(string message, string tag, LogFormatter.Color color, params object[] args);

	void LogWarning(string message, string tag, LogFormatter.Color color, params object[] args);

	void LogError(string message, string tag, LogFormatter.Color color, params object[] args);

	public void LogDev(FormattableString message, string tag, LogFormatter.Color color);

	public void LogWarning(FormattableString message, string tag, LogFormatter.Color color);

	public void LogError(FormattableString message, string tag, LogFormatter.Color color);

	void LogException(Exception exception, string tag, LogFormatter.Color color);
}