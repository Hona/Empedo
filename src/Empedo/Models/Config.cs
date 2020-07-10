using Newtonsoft.Json;

namespace Empedo.Models
{
    public class Config
    {
        [JsonProperty("command_prefix")] public string CommandPrefix { get; set; }
    }
}
