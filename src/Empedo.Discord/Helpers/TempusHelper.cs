using System;
using Empedo.Discord.Models;

namespace Empedo.Discord.Helpers
{
    public static class TempusHelper
    {
        private const string TempusUrl = "https://tempus2.xyz";
        
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

        public static string GetSteamId64(string steamId)
        {
            ulong output = 76561197960265728;

            var idParts = steamId.Split(":");

            output += ulong.Parse(idParts[2]) * 2;

            if (idParts[1] == "1")
            {
                output += 1;
            }
            
            return output.ToString();
        }

        public static TimeSpan TicksToTimeSpan(long ticks) => new(ticks * 149998);
        public static Uri GetMapUrl(string name) => new($"{TempusUrl}/maps/" + name);
        public static Uri GetRecordUrl(int id) => new($"{TempusUrl}/records/" + id);
        public static Uri GetPlayerUrl(int id) => new($"{TempusUrl}/players/" + id);
        public static Uri GetDemoUrl(int id) => new($"{TempusUrl}/demos/" + id);
        public static Uri GetServerUrl(int id) => new($"{TempusUrl}/servers/" + id);
        public static Uri GetYoutubeUrl(string id) => new Uri("https://youtube.com/watch?v=" + id);
        
        public static string GetMapImageUrl(string mapName) =>
            $"https://static.tempus2.xyz/web/screenshots/raw/{mapName}_1080p.jpeg";
    }
}