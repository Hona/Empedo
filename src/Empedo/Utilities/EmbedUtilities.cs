using Discord;

namespace Empedo.Utilities
{
    public static class EmbedUtilities
    {
        public static EmbedBuilder NewWarningEmbed(string text) =>
            new EmbedBuilder
            {
                Description = text
            }.WithCurrentTimestamp();
    }
}