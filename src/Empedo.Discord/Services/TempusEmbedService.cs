using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Empedo.Discord.Helpers;
using Empedo.Discord.Models;
using TempusApi;
using TempusApi.Models.Rank;
using TempusApi.Models.Responses;

namespace Empedo.Discord.Services
{
    [BotService(BotServiceType.Inject)]
    public class TempusEmbedService
    {
        private readonly Tempus _tempus;

        public TempusEmbedService(Tempus tempus)
        {
            _tempus = tempus;
        }

        public async Task<List<DiscordEmbedBuilder>> GetServerOverviewAsync(List<ServerStatusModel> servers = null)
        {
            servers ??= await _tempus.GetServerStatusAsync();

            servers = servers.Where(x => x.GameInfo != null && x.GameInfo.PlayerCount > 0)
                .OrderByDescending(x => x.GameInfo.PlayerCount).ToList();

            var lines = servers.Select(x =>
                $"{x.GameInfo.PlayerCount}/{x.GameInfo.MaxPlayers} • [{x.ServerInfo.Name}](https://tempus.xyz/servers/{x.ServerInfo.Id})").ToList();

            var embedGroups = lines.SplitEmbedDescription();
            var output = new List<DiscordEmbedBuilder>();
            
            for (var i = 0; i < embedGroups.Count; i++)
            {
                var embedGroup = embedGroups[i];
                
                var embedBuilder = new DiscordEmbedBuilder
                {
                    Timestamp = DateTimeOffset.Now,
                    Description = string.Join(Environment.NewLine, embedGroup)
                };

                if (i == 0)
                {
                    embedBuilder.Title = "Server Overview";
                }
                
                output.Add(embedBuilder);
            }

            return output;
        }

        public async Task<List<DiscordEmbedBuilder>> GetTopPlayersOnlineAsync(List<ServerStatusModel> servers = null)
        {
            servers ??= await _tempus.GetServerStatusAsync();

            var users = servers
                .Where(x => x.GameInfo != null &&
                            (x.GameInfo != null || x.ServerInfo != null ||
                             x.GameInfo.Users != null) &&
                            x.GameInfo.Users.Count != 0)
                .SelectMany(x => x.GameInfo.Users)
                .Where(x => x?.Id != null).ToList();

            var userIdStrings = (users.Where(user => user?.Id != null).Select(user => user.Id.ToString())).ToList();

            var rankTasks = new List<Task<Rank>>();
            rankTasks.AddRange(userIdStrings.Select(_tempus.GetUserRankAsync));

            var ranks = await Task.WhenAll(rankTasks);
            var rankedUsers = ranks.ToDictionary(rank => users.First(x => x.Id == rank.PlayerInfo.Id), rank =>
                rank.ClassRankInfo.DemoRank.Rank <= rank.ClassRankInfo.SoldierRank.Rank
                    ? rank.ClassRankInfo.DemoRank.Rank
                    : rank.ClassRankInfo.SoldierRank.Rank);

            rankedUsers = rankedUsers.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            
            var rankedLines = new List<string>();
            foreach (var (key, value) in rankedUsers)
            {
                if (key == null) continue;
                
                var server = servers
                    .FirstOrDefault(x =>
                        x.GameInfo?.Users != null &&
                        x.GameInfo.Users.Any(z => z.Id.HasValue && z.Id == key.Id));
                
                if (server == null || key.Id == null) continue;
                
                rankedLines.Add(
                    $"Rank {value} - {Formatter.MaskedUrl(Formatter.Sanitize(key.Name), TempusHelper.GetPlayerUrl(key.Id.Value))} on {Formatter.MaskedUrl(Formatter.Sanitize(server.GameInfo.CurrentMap), TempusHelper.GetMapUrl(server.GameInfo.CurrentMap))} {Formatter.MaskedUrl(server.ServerInfo.Shortname, TempusHelper.GetServerUrl(server.ServerInfo.Id))}");
            }

            var rankedLineGroups = rankedLines.SplitEmbedDescription();
            var output = new List<DiscordEmbedBuilder>();

            for (var i = 0; i < rankedLineGroups.Count; i++)
            {
                var lineGroup = rankedLineGroups[i];
                
                var builder = new DiscordEmbedBuilder
                {
                    Description = string.Join(Environment.NewLine, lineGroup),
                    Timestamp = DateTimeOffset.Now
                };

                if (i == 0)
                {
                    builder.Title = "**Highest Ranked Players Online**";
                }

                output.Add(builder);
            }

            return output;
        }
    }
}