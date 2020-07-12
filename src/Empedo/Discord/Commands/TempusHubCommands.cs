using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Empedo.Models;
using Empedo.Services;
using Empedo.Utilities;
using Environment = System.Environment;

namespace Empedo.Discord.Commands
{
    public class TempusHubCommands : ModuleBase
    {
        private readonly int _resultsPerPage = 7;
        public Config Config { get; set; }

        [Command("stalktop")]
        public async Task TopPlayersOnlineAsync(int page = 1)
        {
            if (page < 1)
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("Page number must be 1 or greater").Build());
                return;
            }

            var topPlayersOnline = await TempusHubDataService.GetTopPlayersOnlineAsync();

            var desiredPlayers = topPlayersOnline.Skip(_resultsPerPage * (page - 1)).Take(_resultsPerPage).ToList();
            var pageCount = Math.Ceiling((double) topPlayersOnline.Count / _resultsPerPage);

            if (desiredPlayers.Any())
            {
                var embedBuilder = new EmbedBuilder
                {
                    Title = "Top Players Online",
                    Description = string.Join(Environment.NewLine, desiredPlayers.Select(x =>
                        $"{(x.RankClass == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} Rank {x.Rank} {Format.Url(Format.Bold(Format.Sanitize(x.RealName ?? x.SteamName)), TempusHubHelper.PlayerUrl(x.TempusId))} on {Format.Url(Format.Bold(Format.Sanitize(x.ServerInfo.CurrentMap)), TempusHubHelper.MapUrl(x.ServerInfo.CurrentMap))} • {Format.Url(x.ServerInfo.Alias, TempusHubHelper.ServerUrl(x.ServerInfo.Id))}"))
                }.WithFooter($"Page {page} of {pageCount}").WithCurrentTimestamp();

                await ReplyAsync(embed: embedBuilder.Build());
            }
            else
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("No more results.").Build());
            }
        }
    }
}