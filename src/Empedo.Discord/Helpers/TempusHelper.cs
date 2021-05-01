using System;
using Empedo.Discord.Models;

namespace Empedo.Discord.Helpers
{
    public static class TempusHelper
    {
        public static string GetClass(int id)
        {
            switch (id)
            {
                case 4:
                    return "D";
                case 3:
                    return "S";
                default:
                    return id.ToString();
            }
        }

        public static string GetClassEmote(int id)
        {
            var classString = GetClass(id);

            return classString == "S" ? "<:soldier:702841577806233702>" : " <:demoman:702841578259087380>";
        }

        public static string GetEmote(this TempusClass tempusClass) => GetClassEmote((int) tempusClass);

        public static string TicksToFormattedTime(long ticks)
        {
            var timeSpan = TicksToTimeSpan(ticks);
            return TimeSpanToFormattedTime(timeSpan);
        }

        public static string TimeSpanToFormattedTime(TimeSpan timeSpan)
        {
            var factor = (int) Math.Pow(10, 5);
            var roundedTimeSpan = new TimeSpan((long) Math.Round(1.0 * timeSpan.Ticks / factor) * factor);
            return
                $"{roundedTimeSpan.Days}:{roundedTimeSpan.Hours}:{roundedTimeSpan.Minutes}:{roundedTimeSpan.Seconds}.{Math.Round((double) roundedTimeSpan.Milliseconds)}"
                    .Trim('0', ':', '.');
        }

        public static TimeSpan TicksToTimeSpan(long ticks) => new(ticks * 149998);
        public static Uri GetMapUrl(string name) => new("https://tempus.xyz/maps/" + name);
        public static Uri GetRecordUrl(int id) => new("https://tempus.xyz/records/" + id);
        public static Uri GetPlayerUrl(int id) => new("https://tempus.xyz/players/" + id);
        public static Uri GetDemoUrl(int id) => new("https://tempus.xyz/demos/" + id);
        public static Uri GetServerUrl(int id) => new("https://tempus.xyz/servers/" + id);
        public static Uri GetYoutubeUrl(string id) => new Uri("https://youtube.com/watch?v=" + id);
    }
}