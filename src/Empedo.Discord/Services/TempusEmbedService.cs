﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Empedo.Discord.Helpers;
using Empedo.Discord.Models;
using TempusApi;
using TempusApi.Models.Activity;
using TempusApi.Models.Rank;
using TempusApi.Models.Responses;

namespace Empedo.Discord.Services
{
    [BotService(BotServiceType.Inject)]
    public class TempusEmbedService : ITempusEmbedService
    {
        private readonly Tempus _tempus;
        
        public TempusEmbedService(Tempus tempus)
        {
            _tempus = tempus;
        }

        public async Task<List<DiscordEmbedBuilder>> GetServerOverviewAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false)
        {
            servers ??= await _tempus.GetServerStatusAsync();

            servers = servers.Where(x => x.GameInfo != null && x.GameInfo.PlayerCount > 0)
                .OrderByDescending(x => x.GameInfo.PlayerCount).ToList();
            
            return servers.BuildEmbeds(x =>
                    $"`{x.GameInfo.PlayerCount.ToString().PadLeft(2)}/{x.GameInfo.MaxPlayers.ToString().PadLeft(2)}` • [{x.ServerInfo.Name}](https://tempus.xyz/servers/{x.ServerInfo.Id}) • {Formatter.MaskedUrl(x.GameInfo.CurrentMap, TempusHelper.GetMapUrl(x.GameInfo.CurrentMap))}",x => x.Title = "Server Overview",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }

        public async Task<List<DiscordEmbedBuilder>> GetTopPlayersOnlineAsync(List<ServerStatusModel> servers = null, bool decorateAllEmbeds = false)
        {
            servers ??= await _tempus.GetServerStatusAsync();

            var users = servers
                .Where(x => x.GameInfo != null &&
                            (x.GameInfo != null || x.ServerInfo != null ||
                             x.GameInfo.Users != null) &&
                            x.GameInfo.Users.Count != 0)
                .SelectMany(x => x.GameInfo.Users)
                .Where(x => x?.Id != null).ToList();

            var userIdStrings = users
                .Where(user => user?.Id != null)
                // TODO: Investigate this after TempusHub fixes 0 IDs
                .Where(user => user.Id is not 0)
                .Select(user => user.Id.ToString()).ToList();

            var rankTasks = new List<Task<Rank>>();
            rankTasks.AddRange(userIdStrings.Select(_tempus.GetUserRankAsync));

            var ranks = await Task.WhenAll(rankTasks);
            
            var rankedUsers = ranks.ToDictionary(rank => users.First(x => x.Id == rank.PlayerInfo.Id),
                rank =>
                    rank.ClassRankInfo.DemoRank.Rank <= rank.ClassRankInfo.SoldierRank.Rank
                        ? new ClassRankViewModel
                        {
                            Class = TempusClass.Demoman,
                            Rank = rank.ClassRankInfo.DemoRank.Rank
                        }
                        : new ClassRankViewModel
                        {
                            Class = TempusClass.Soldier,
                            Rank = rank.ClassRankInfo.SoldierRank.Rank
                        });

            rankedUsers = rankedUsers.OrderBy(x => x.Value.Rank)
                .ToDictionary(x => x.Key, x => x.Value);
            
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
                    $"{value.Class.GetEmote()} {value.Rank} • {Formatter.MaskedUrl(key.Name, TempusHelper.GetPlayerUrl(key.Id.Value))} on {Formatter.MaskedUrl(server.GameInfo.CurrentMap, TempusHelper.GetMapUrl(server.GameInfo.CurrentMap))} • {Formatter.MaskedUrl(server.ServerInfo.Shortname, TempusHelper.GetServerUrl(server.ServerInfo.Id))}");
            }

            var rankedLineGroups = rankedLines.SplitEmbedDescription();

            return rankedLineGroups.BuildEmbeds(x => x.Title = "**Top Players Online**",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }

        private static string FormatRecord(MapInfo mapInfo, RecordInfoShort recordInfo, TempusApi.Models.Activity.PlayerInfo playerInfo) =>
            $" • {Formatter.MaskedUrl(playerInfo.Name, TempusHelper.GetPlayerUrl(playerInfo.Id))} on {Formatter.MaskedUrl(mapInfo.Name, TempusHelper.GetMapUrl(mapInfo.Name))} • {Formatter.MaskedUrl($"**{FormatDuration(recordInfo.Duration)}**", TempusHelper.GetRecordUrl(recordInfo.Id))}";
        
        private static string FormatDuration(double duration)
        {
            var seconds = (int)Math.Truncate(duration);
            var milliseconds = (duration - (int)Math.Truncate(duration)) * 1000;
            var timespan = new TimeSpan(0, 0, 0, seconds, (int)Math.Truncate(milliseconds));
            return timespan.Days > 0 ? timespan.ToString(@"dd\:hh\:mm\:ss\.ff") : timespan.ToString(timespan.Hours > 0 ? @"hh\:mm\:ss\.ff" : @"mm\:ss\.ff");
        }
        
        public async Task<List<DiscordEmbedBuilder>> GetRecentMapRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
        {
            activity ??= await _tempus.GetRecentActivityAsync();

            return activity.MapRecords.BuildEmbeds(x => $"{TempusHelper.GetClassEmote(x.RecordInfo.Class)}"
                                                        + FormatRecord(x.MapInfo, x.RecordInfo, x.PlayerInfo),
                x => x.Title = "Recent Map Records",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }
        
        public async Task<List<DiscordEmbedBuilder>> GetRecentCourseRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
        {
            activity ??= await _tempus.GetRecentActivityAsync();

            return activity.CourseRecords.BuildEmbeds(x => $"{TempusHelper.GetClassEmote(x.RecordInfo.Class)} C{x.ZoneInfo.Zoneindex}"
                                                        + FormatRecord(x.MapInfo, x.RecordInfo, x.PlayerInfo),
                x => x.Title = "Recent Course Records",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }
        
        public async Task<List<DiscordEmbedBuilder>> GetRecentBonusRecordsAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
        {
            activity ??= await _tempus.GetRecentActivityAsync();

            return activity.BonusRecords.BuildEmbeds(x => $"{TempusHelper.GetClassEmote(x.RecordInfo.Class)} B{x.ZoneInfo.Zoneindex}"
                                                           + FormatRecord(x.MapInfo, x.RecordInfo, x.PlayerInfo),
                x => x.Title = "Recent Bonus Records",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }
        
        public async Task<List<DiscordEmbedBuilder>> GetRecentMapTopTimesAsync(RecentActivityModel activity = null, bool decorateAllEmbeds = false)
        {
            activity ??= await _tempus.GetRecentActivityAsync();

            return activity.MapTopTimes.BuildEmbeds(x => $"{TempusHelper.GetClassEmote(x.RecordInfo.Class)} #{x.Rank}"
                                                           + FormatRecord(x.MapInfo, x.RecordInfo, x.PlayerInfo),
                x => x.Title = "Recent Map Top Times",
                x => x.Timestamp = DateTimeOffset.Now, decorateAllEmbeds);
        }

        public async Task<List<DiscordEmbedBuilder>> GetServerListAsync(List<ServerStatusModel> servers = null)
        {
            servers ??= await _tempus.GetServerStatusAsync();

            var output = new List<DiscordEmbedBuilder>();

            foreach (var server in servers.OrderByDescending(x => x.GameInfo != null)
                .ThenByDescending(x => x.GameInfo?.PlayerCount ?? -1))
            {
                if (server?.ServerInfo == null || server.GameInfo == null || server.ServerInfo.Hidden ||
                    server.GameInfo.PlayerCount == 0)
                    continue;

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"**{server.ServerInfo.Name}**"
                };
                
                embed.AddField("Map", Formatter.MaskedUrl(server.GameInfo.CurrentMap, TempusHelper.GetMapUrl(server.GameInfo.CurrentMap)));
                
                if (server.GameInfo.NextMap != null)
                    embed.AddField("Next Map", Formatter.MaskedUrl(server.GameInfo.NextMap.ToString(), TempusHelper.GetMapUrl(server.GameInfo.NextMap.ToString())));

                embed.AddField("Connect",
                    Formatter.Sanitize(server.ServerInfo.Addr));

                embed.AddField("Players Online", server.GameInfo.PlayerCount + "/" + server.GameInfo.MaxPlayers);
                
                if (server.GameInfo.Users.Any())
                    embed.AddField("Players",
                        server.GameInfo.Users.OrderBy(x => x.Name).Aggregate("",
                                (currentString, nextPlayer) => currentString + (nextPlayer.Id.HasValue ? Formatter.MaskedUrl(nextPlayer.Name, TempusHelper.GetPlayerUrl(nextPlayer.Id.Value)) : nextPlayer.Name)  + ", ")
                            .TrimEnd(',', ' '));
                
                output.Add(embed);
            }

            return output;
        }

        public async Task<List<DiscordEmbedBuilder>> GetMapOverviewAsync(string mapName)
        {
            var map = await _tempus.GetFullMapOverViewAsync(mapName);
            var firstAuthorSteamId = TempusHelper.GetSteamId64(map.Authors.First().SteamId);
            var authorAvatar = await _tempus.GetSteamAvatarsAsync(firstAuthorSteamId);
            
            var output = new List<DiscordEmbedBuilder>();
            
            output.Add(new DiscordEmbedBuilder
            {
                Title = map.MapInfo.Name,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = authorAvatar.FirstOrDefault().Value?.Avatars?.MediumUrl,
                    Name = map.Authors.First().Name + (map.Authors.Count == 1 ? string.Empty : $" + {map.Authors.Count - 1} others"),
                    Url = TempusHelper.GetPlayerUrl(map.Authors.First().UserId).ToString()
                },
                Timestamp = DateTimeOffset.FromUnixTimeSeconds((long)map.MapInfo.DateAdded),
                Description = $"{TempusHelper.GetClassEmote(3)} T{map.TierInfo.Soldier} {Formatter.MaskedUrl("Showcase", TempusHelper.GetYoutubeUrl(map.Videos.Soldier))} • {TempusHelper.GetClassEmote(4)} T{map.TierInfo.Demoman} {Formatter.MaskedUrl("Showcase", TempusHelper.GetYoutubeUrl(map.Videos.Demoman))}",
                ImageUrl = TempusHelper.GetMapImageUrl(map.MapInfo.Name)
            });

            return output;
        }
    }
}