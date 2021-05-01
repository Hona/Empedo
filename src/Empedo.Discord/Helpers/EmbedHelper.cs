﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Empedo.Discord.Helpers
{
    public static class EmbedHelper
    {
        public static List<List<string>> SplitEmbedDescription(this List<string> lines, int lengthAllowed = 2048)
        {
            var totalCount = 0;

            var output = new List<List<string>>();

            var currentEmbed = new List<string>();
            
            foreach (var line in lines)
            {
                // Include the newline in the count
                totalCount += line.Length + 1;

                if (totalCount > lengthAllowed)
                {
                    // Make a copy
                    output.Add(currentEmbed.ToList());
                    currentEmbed = new List<string>();
                }
                
                currentEmbed.Add(line);
            }

            return output;
        }

        public static async Task SendAll(this IEnumerable<DiscordEmbedBuilder> embedBuilders, DiscordChannel discordChannel)
        {
            foreach (var embedBuilder in embedBuilders)
            {
                await discordChannel.SendMessageAsync(embedBuilder);
            }
        }
    }
}