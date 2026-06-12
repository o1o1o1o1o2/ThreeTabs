using System;
using System.Diagnostics;
using System.Threading;
using Shared.Libs.Utils;

// ReSharper disable once CheckNamespace
public class LogCat<T> : LogCat
{
    public LogCat(LogFormatter.Color? color = null) : base(typeof(T).Name, color)
    {
    }
}

public class LogCat
{
    private readonly string _tag;
    private readonly LogFormatter.Color _color;

    private static int _nextColor = -1;

    private static ILogger _logger;

    public static ILogger Logger
    {
        private get
        {
#if UNITY3D
			return _logger ??= new Shared.Libs.Logging.Unity3DLogger();
#else
            return _logger;
#endif
        }
        set
        {
            _logger = value;
        }
    }

    public LogCat(string tag, LogFormatter.Color? color = null)
    {
        _tag = tag;
        _color = color ?? NextColor();
    }

    private static LogFormatter.Color NextColor()
    {
        var colorIndex = Interlocked.Increment(ref _nextColor);
        var colors = Enums.Values<LogFormatter.Color>();
        return colors[colorIndex % colors.Length];
    }

    [Conditional("DEVELOPMENT_BUILD")]
    public void LogDev(string message, params object[] args) =>
        Logger?.LogDev(message, _tag, _color, args);

    public void LogInfo(string message, params object[] args) =>
        Logger?.LogInfo(message, _tag, _color, args);

    public void LogWarning(string message, params object[] args) =>
        Logger?.LogWarning(message, _tag, _color, args);

    public void LogError(string message, params object[] args) =>
        Logger?.LogError(message, _tag, _color, args);

    [Conditional("DEVELOPMENT_BUILD")]
    public void LogDev(FormattableString message) =>
        Logger?.LogDev(message, _tag, _color);

    public void LogWarning(FormattableString message) =>
        Logger?.LogWarning(message, _tag, _color);

    public void LogError(FormattableString message) =>
        Logger?.LogError(message, _tag, _color);

    public void LogException(Exception ex) =>
        Logger?.LogException(ex, _tag, _color);
}