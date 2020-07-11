using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace Empedo.Utilities
{
    public static class EmbedUtilities
    {
        public static EmbedBuilder NewWarningEmbed(string text)
        {
            return new EmbedBuilder
            {
                Description = text
            }.WithCurrentTimestamp();
        }
    }
}
