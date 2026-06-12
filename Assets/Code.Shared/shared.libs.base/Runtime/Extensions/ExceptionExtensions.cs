// ReSharper disable CheckNamespace

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

public static class ExceptionExtensions
{
    private const int MAX_INNER_EXCEPTION_COUNT = 4;

    public static string ToLog(this Exception ex, string startsWith = null, bool showStackTrace = true)
    {
        var sb = new StringBuilder(1024);

        if (startsWith != null)
            sb.AppendLine(startsWith);
        else
        {
            var method = new StackTrace().GetFrame(1)?.GetMethod();
            if (method != null)
                sb.AppendLine($"{method.DeclaringType?.Name ?? "[NaN]"}.{method.Name}");
        }

        sb.AppendLine($"----- {0:00} {ex.GetType().Name}: {ex.Message}");
        if (showStackTrace)
            sb.AppendLine(ex.StackTrace);
        else
            sb.AppendLine(ex.StackTrace?.Split(Environment.NewLine).FirstOrDefault());

        var innerException = ex.InnerException;
        var index = 1;
        while (innerException != null && index < MAX_INNER_EXCEPTION_COUNT)
        {
            sb.AppendLine($"----- {index:00} {innerException.GetType().Name}: {innerException.Message}");

            innerException = innerException.InnerException;
            ++index;
        }

        innerException = ex.InnerException;
        index = 1;
        while (showStackTrace && innerException != null && index < MAX_INNER_EXCEPTION_COUNT)
        {
            sb.AppendLine($"----- {index:00} {innerException.GetType().Name}: {innerException.Message}");
            sb.AppendLine(innerException.StackTrace);

            innerException = innerException.InnerException;
            ++index;
        }

        if (index >= MAX_INNER_EXCEPTION_COUNT)
            sb.AppendLine($"!!!WARNING EXCEPTION '{ex.GetType().Name}' HAVE MORE THAN {index} INNER EXCEPTIONS!!!");

        return sb.ToString();
    }

    /// <summary>
    /// Returns the object if it is not null, otherwise throws ArgumentNullException.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ThrowIfNull<T>(this T obj, string paramName) where T : class =>
        obj ?? throw new ArgumentNullException(paramName, $"'{paramName}' is null <{typeof(T).Name}>.");

    public static void LogIfNotOperationCanceled(this Exception exception)
    {
        if (exception is OperationCanceledException)
            return;
        StaticLogger.LogError(exception.ToLog());
    }
}