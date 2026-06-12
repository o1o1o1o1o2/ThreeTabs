#if UNITY_3D
using System;
using UnityEngine;

namespace Shared.Libs.Logging {

	public class Unity3DLogger : ILogger {
		public void LogTrace(string message, string tag, LogFormatter.Color color, params object[] args) {
			if (!args.IsNullOrEmpty()) message = string.Format(message, args);
			Debug.Log($"[{tag.WithColor(color)}] {message}");
		}

		public void LogDev(string message, string tag, LogFormatter.Color color, params object[] args) {
			if (!args.IsNullOrEmpty()) message = string.Format(message, args);
			Debug.Log($"[{tag.WithColor(color)}] {message}");
		}

		public void LogInfo(string message, string tag, LogFormatter.Color color, params object[] args) {
			if (!args.IsNullOrEmpty()) message = string.Format(message, args);
			Debug.Log($"[{tag.WithColor(color)}] {message}");
		}

		public void LogWarning(string message, string tag, LogFormatter.Color color, params object[] args) {
			if (!args.IsNullOrEmpty()) message = string.Format(message, args);
			Debug.LogWarning($"[{tag.WithColor(color)}] {message}");
		}

		public void LogError(string message, string tag, LogFormatter.Color color, params object[] args) {
			if (!args.IsNullOrEmpty()) message = string.Format(message, args);
			Debug.LogError($"[{tag.WithColor(color)}] {message}");
		}

		public void LogDev(FormattableString message, string tag, LogFormatter.Color color) {
			Debug.Log($"[{tag.WithColor(color)}] {message.ToString(LogFormatter.Default)}");
		}

		public void LogWarning(FormattableString message, string tag, LogFormatter.Color color) {
			Debug.LogWarning($"[{tag.WithColor(color)}] {message.ToString(LogFormatter.Default)}");
		}

		public void LogError(FormattableString message, string tag, LogFormatter.Color color) {
			Debug.LogError($"[{tag.WithColor(color)}] {message.ToString(LogFormatter.Default)}");
		}

		public void LogException(Exception exception, string tag, LogFormatter.Color color) {
			Debug.LogError(exception.ToLog($"[{tag.WithColor(color)}] "));
		}
	}

}
#endif