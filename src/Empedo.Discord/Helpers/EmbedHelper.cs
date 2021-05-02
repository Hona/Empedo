using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace Empedo.Discord.Helpers
{
    public static class EmbedHelper
    {
        public static List<List<string>> SplitEmbedDescription(this List<string> lines, int lengthAllowed = 2048)
        {
            var output = new List<List<string>>();

            var currentEmbedLength = 0;
            var currentEmbed = new List<string>();
            
            foreach (var line in lines)
            {
                // Include the newline in the count
                var lineLength = line.Length + 2;
                currentEmbedLength += lineLength;

                if (currentEmbedLength > lengthAllowed)
                {
                    // Make a copy
                    output.Add(currentEmbed.ToList());
                    currentEmbed = new List<string>();
                    currentEmbedLength = lineLength;
                }
                
                currentEmbed.Add(line);
            }

            if (currentEmbed.Any())
            {
                output.Add(currentEmbed);
            }

            Console.WriteLine(string.Join(", ", output.Select(x => $"{x.Sum(x => x.Length)} ({x.Count})")));
            
            return output;
        }

        public static async Task SendAll(this IEnumerable<DiscordEmbedBuilder> embedBuilders, DiscordChannel discordChannel)
        {
            foreach (var embedBuilder in embedBuilders)
            {
                await discordChannel.SendMessageAsync(embedBuilder);
            }
        }

        public static List<DiscordEmbedBuilder> BuildEmbeds(this List<List<string>> lineGroups,
            Action<DiscordEmbedBuilder> firstEmbedAction, Action<DiscordEmbedBuilder> lastEmbedAction,
            bool runActionsEveryTime = false, string joinStringOverride = null)
        {
            var output = new List<DiscordEmbedBuilder>();

            for (var i = 0; i < lineGroups.Count; i++)
            {
                var lineGroup = lineGroups[i];
                
                var embed = new DiscordEmbedBuilder
                {
                    Description = string.Join(joinStringOverride ?? Environment.NewLine, lineGroup)
                };

                if (i == 0 || runActionsEveryTime)
                {
                    firstEmbedAction.Invoke(embed);
                }

                if (i == lineGroups.Count - 1 || runActionsEveryTime)
                {
                    lastEmbedAction.Invoke(embed);
                }

                output.Add(embed);
            }

            return output;
        }

        public static List<DiscordEmbedBuilder> BuildEmbeds<T>(this IEnumerable<T> model, Func<T, string> selector,
            Action<DiscordEmbedBuilder> firstEmbedAction, Action<DiscordEmbedBuilder> lastEmbedAction,
            bool runActionsEveryTime = false, string joinStringOverride = null)
        {
            var lines = model.Select(selector).ToList();

            var lineGroups = lines.SplitEmbedDescription();

            return lineGroups.BuildEmbeds(firstEmbedAction, lastEmbedAction, runActionsEveryTime, joinStringOverride);
        }
    }
}