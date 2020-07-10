using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Empedo.Models;
using Empedo.Services;
using Environment = System.Environment;

namespace Empedo.Discord.Commands
{
    public class TempusHubCommands : ModuleBase
    {
        public Config Config { get; set; }

        [Command("stalktop")]
        public async Task TopPlayersOnlineAsync()
        {
            var topPlayersOnline = await TempusHubDataService.GetTopPlayersOnlineAsync();

            var embedBuilder = new EmbedBuilder
            {
                Title = "Top Players Online",
                Description = string.Join(Environment.NewLine, topPlayersOnline.Take(7).Select(x =>
                    $"{(x.RankClass == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} Rank {x.Rank} | {x.RealName ?? x.SteamName} | {x.ServerInfo.CurrentMap} | {Format.Url(x.ServerInfo.Alias, $"https://tempushub.xyz/server/" + x.ServerInfo.Id)}"))
            }.WithCurrentTimestamp();

            await ReplyAsync(embed: embedBuilder.Build());
        }
    }
}
