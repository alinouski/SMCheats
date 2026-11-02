using System.Globalization;
using System.Text;
using UnityEngine;

namespace SMCheats
{
    internal static class CheatsConsoleEntryFormatter
    {
        public static string FormatDetails(CheatsConsoleEntry entry)
        {
            var builder = new StringBuilder();
            AppendDetails(builder, entry);
            return builder.ToString();
        }

        public static string FormatRowMessage(CheatsConsoleEntry entry)
        {
            var suffix = entry.Count > 1
                ? " x" + entry.Count.ToString(CultureInfo.InvariantCulture)
                : string.Empty;

            return TrimLine(entry.Message, 180) + suffix;
        }

        public static string FormatRowStackTrace(CheatsConsoleEntry entry)
        {
            return TrimLine(entry.StackTrace, 180);
        }

        private static void AppendDetails(StringBuilder builder, CheatsConsoleEntry entry)
        {
            builder.Append(entry.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
            builder.Append(" [");
            builder.Append(entry.LogType);
            builder.Append("] frame ");
            builder.Append(entry.FrameCount.ToString(CultureInfo.InvariantCulture));
            builder.Append(" scene ");
            builder.AppendLine(string.IsNullOrWhiteSpace(entry.SceneName) ? "Unknown" : entry.SceneName);
            builder.Append("Repeat: ");
            builder.AppendLine(entry.Count.ToString(CultureInfo.InvariantCulture));
            builder.AppendLine(entry.Message);

            if (!string.IsNullOrWhiteSpace(entry.StackTrace))
            {
                builder.AppendLine(entry.StackTrace);
            }
        }

        private static string TrimLine(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var carriageReturnIndex = value.IndexOf('\r');
            var lineFeedIndex = value.IndexOf('\n');
            var firstLineBreak = carriageReturnIndex < 0
                ? lineFeedIndex
                : lineFeedIndex < 0
                    ? carriageReturnIndex
                    : Mathf.Min(carriageReturnIndex, lineFeedIndex);
            var line = firstLineBreak >= 0 ? value.Substring(0, firstLineBreak) : value;
            return line.Length <= maxLength ? line : line.Substring(0, maxLength);
        }
    }
}
