using System;
using System.Text;

namespace Empedo.Utilities
{
    public static class RecordUtilities
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);
        public static DateTime GetDateFromSeconds(double seconds) => UnixEpoch.AddSeconds(seconds);

        public static string GetPrettyTimeSince(this double seconds) =>
            GetDateFromSeconds(seconds).GetTimeStringSinceDateTime();

        public static string GetTimeStringSinceDateTime(this DateTime dateTime)
        {
            TimeSpan deltaTime;
            if (dateTime.Ticks < DateTime.Now.Ticks)
            {
                deltaTime = DateTime.Now - dateTime;
                return deltaTime.ToPrettyFormat() + " ago";
            }

            deltaTime = dateTime - DateTime.Now;
            return deltaTime.ToPrettyFormat() + " in the future";
        }

        private static string ToPrettyFormat(this TimeSpan span)
        {
            if (span == TimeSpan.Zero)
            {
                return "0 minutes";
            }

            var sb = new StringBuilder();
            if (span.Days > 0)
            {
                return sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : string.Empty).ToString();
            }

            if (span.Hours > 0)
            {
                return sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : string.Empty).ToString();
            }

            if (span.Minutes > 0)
            {
                return sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : string.Empty)
                    .ToString();
            }

            return sb.ToString();
        }

        public static string FormattedDuration(double? duration)
        {
            if (!duration.HasValue)
            {
                return null;
            }

            var seconds = (int) Math.Truncate(duration.Value);
            var milliseconds = (duration.Value - (int) Math.Truncate(duration.Value)) * 1000;
            var timespan = new TimeSpan(0, 0, 0, seconds, (int) Math.Truncate(milliseconds));
            return timespan.Days > 0
                ? timespan.ToString(@"dd\:hh\:mm\:ss\.fff")
                : timespan.ToString(timespan.Hours > 0 ? @"hh\:mm\:ss\.fff" : @"mm\:ss\.fff");
        }
    }
}