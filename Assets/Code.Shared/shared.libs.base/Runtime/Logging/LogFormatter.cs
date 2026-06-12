using System;
using System.Collections.Generic;
using Shared.Libs.Utils;

// ReSharper disable once CheckNamespace
public static class LogFormatter
{
    public enum Color
    {
        green,
        yellow,
        red,
        blue,
        magenta,
        cyan,
        white,
        darkred,
        darkgreen,
        darkmagenta,
        darkcyan,
        darkyellow,
        darkblue,
        gray
    }

    public interface IRichFormatter : IFormatProvider, ICustomFormatter
    {
    }

    public static readonly IRichFormatter Default = new UnityRichLog();

    public class UnityRichLog : IRichFormatter
    {
        private static readonly IReadOnlyDictionary<string, uint> Foreground = new Dictionary<string, uint>
        {
            { nameof(Color.darkred), 0x800000 },
            { nameof(Color.darkgreen), 0x008000 },
            { nameof(Color.darkyellow), 0x808000 },
            { nameof(Color.darkblue), 0x000080 },
            { nameof(Color.darkmagenta), 0x800080 },
            { nameof(Color.darkcyan), 0x008080 },
            { nameof(Color.gray), 0xC0C0C0 },
            { nameof(Color.red), 0xFF0000 },
            { nameof(Color.green), 0x00FF00 },
            { nameof(Color.yellow), 0xFFFF00 },
            { nameof(Color.blue), 0x0000FF },
            { nameof(Color.magenta), 0xFF00FF },
            { nameof(Color.cyan), 0x00FFFF },
            { nameof(Color.white), 0xFFFFFF }
        };

        object IFormatProvider.GetFormat(Type formatType) => TypeOf<ICustomFormatter>.Raw == formatType ? this : null;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null)
                return arg?.ToString();

            if (Foreground.TryGetValue(format, out var color))
                return $"<color=#{color:X6}>{arg}</color>";

            format = $"{{0:{format}}}";
            return string.Format(format, arg);
        }
    }

    public static string WithColor(this string message, Color color)
    {
#if UNITY3D
		return Default.Format(color.ToString(), message, null);
#else
        return message;
#endif
    }
}