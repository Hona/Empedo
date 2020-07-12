using Discord;
using Newtonsoft.Json;

namespace Empedo.Models
{
    public class Config
    {
        [JsonProperty("command_prefix")] public string CommandPrefix { get; set; }
        [JsonProperty("demoman_emoji")] public string DemomanEmojiString { get; set; }
        [JsonProperty("soldier_emoji")] public string SoldierEmojiString { get; set; }
        [JsonIgnore] public Emoji DemomanEmoji => new Emoji(DemomanEmojiString);
        [JsonIgnore] public Emoji SoldierEmoji => new Emoji(SoldierEmojiString);
    }
}