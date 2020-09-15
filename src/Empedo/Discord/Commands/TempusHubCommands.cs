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

        [Command("rr")]
        public async Task MapWRAsync(int page = 1)
        {
            if (page < 1)
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("Page number must be 1 or greater").Build());
                return;
            }

            var mapWRs = (await TempusHubDataService.GetRecentMapWRsAsync()).OrderByDescending(x => x.RecordInfo.Date)
                .ToList();

            var desiredWRs = mapWRs.Skip(_resultsPerPage * (page - 1)).Take(_resultsPerPage).ToList();
            var pageCount = Math.Ceiling((double) mapWRs.Count / _resultsPerPage);

            if (desiredWRs.Any())
            {
                var embedBuilder = new EmbedBuilder
                {
                    Title = "Recent Map WRs",
                    Description = string.Join(Environment.NewLine, desiredWRs.Select(x =>
                        $"{(x.RecordInfo.Class == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} WR • {Format.Url(Format.Bold(Format.Sanitize(x.PlayerInfo.RealName ?? x.PlayerInfo.Name)), TempusHubHelper.PlayerUrl(x.PlayerInfo.Id))} on {Format.Url(Format.Bold(Format.Sanitize(x.MapInfo.Name)), TempusHubHelper.MapUrl(x.MapInfo.Name))} • {Format.Bold(RecordUtilities.FormattedDuration(x.RecordInfo.Duration))} (WR -{RecordUtilities.FormattedDuration(x.CachedTime.OldWRDuration - x.CachedTime.CurrentWRDuration) ?? "N/A"}) • {x.RecordInfo.Date.GetPrettyTimeSince()}"))
                }.WithFooter($"Page {page} of {pageCount}").WithCurrentTimestamp();

                await ReplyAsync(embed: embedBuilder.Build());
            }
            else
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("No more results.").Build());
            }
        }

        [Command("rrc")]
        public async Task CourseWRAsync(int page = 1)
        {
            if (page < 1)
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("Page number must be 1 or greater").Build());
                return;
            }

            var courseWRs = (await TempusHubDataService.GetRecentCourseWRsAsync())
                .OrderByDescending(x => x.RecordInfo.Date).ToList();

            var desiredWRs = courseWRs.Skip(_resultsPerPage * (page - 1)).Take(_resultsPerPage).ToList();
            var pageCount = Math.Ceiling((double) courseWRs.Count / _resultsPerPage);

            if (desiredWRs.Any())
            {
                var embedBuilder = new EmbedBuilder
                {
                    Title = "Recent Course WRs",
                    Description = string.Join(Environment.NewLine, desiredWRs.Select(x =>
                        $"{(x.RecordInfo.Class == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} WR • {Format.Url(Format.Bold(Format.Sanitize(x.PlayerInfo.RealName ?? x.PlayerInfo.Name)), TempusHubHelper.PlayerUrl(x.PlayerInfo.Id))} on {Format.Url(Format.Bold(Format.Sanitize(x.MapInfo.Name) + " C" + x.ZoneInfo.ZoneIndex), TempusHubHelper.MapUrl(x.MapInfo.Name))} • {Format.Bold(RecordUtilities.FormattedDuration(x.RecordInfo.Duration))} (WR -{RecordUtilities.FormattedDuration(x.CachedTime.OldWRDuration.Value - x.CachedTime.CurrentWRDuration.Value)}) • {x.RecordInfo.Date.GetPrettyTimeSince()}"))
                }.WithFooter($"Page {page} of {pageCount}").WithCurrentTimestamp();

                await ReplyAsync(embed: embedBuilder.Build());
            }
            else
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("No more results.").Build());
            }
        }

        [Command("rrb")]
        public async Task BonusWRAsync(int page = 1)
        {
            if (page < 1)
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("Page number must be 1 or greater").Build());
                return;
            }

            var bonusWRs = (await TempusHubDataService.GetRecentBonusWRsAsync())
                .OrderByDescending(x => x.RecordInfo.Date).ToList();

            var desiredWRs = bonusWRs.Skip(_resultsPerPage * (page - 1)).Take(_resultsPerPage).ToList();
            var pageCount = Math.Ceiling((double) bonusWRs.Count / _resultsPerPage);

            if (desiredWRs.Any())
            {
                var embedBuilder = new EmbedBuilder
                {
                    Title = "Recent Bonus WRs",
                    Description = string.Join(Environment.NewLine, desiredWRs.Select(x =>
                        $"{(x.RecordInfo.Class == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} WR • {Format.Url(Format.Bold(Format.Sanitize(x.PlayerInfo.RealName ?? x.PlayerInfo.Name)), TempusHubHelper.PlayerUrl(x.PlayerInfo.Id))} on {Format.Url(Format.Bold(Format.Sanitize(x.MapInfo.Name) + " B" + x.ZoneInfo.ZoneIndex), TempusHubHelper.MapUrl(x.MapInfo.Name))} • {Format.Bold(RecordUtilities.FormattedDuration(x.RecordInfo.Duration))} (WR -{RecordUtilities.FormattedDuration(x.CachedTime.OldWRDuration.Value - x.CachedTime.CurrentWRDuration.Value)}) • {x.RecordInfo.Date.GetPrettyTimeSince()}"))
                }.WithFooter($"Page {page} of {pageCount}").WithCurrentTimestamp();

                await ReplyAsync(embed: embedBuilder.Build());
            }
            else
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("No more results.").Build());
            }
        }

        [Command("rrtt")]
        public async Task MapTTAsync(int page = 1)
        {
            if (page < 1)
            {
                await ReplyAsync(embed: EmbedUtilities.NewWarningEmbed("Page number must be 1 or greater").Build());
                return;
            }

            var mapTTs = (await TempusHubDataService.GetRecentMapTTsAsync()).OrderByDescending(x => x.RecordInfo.Date)
                .ToList();

            var desiredWRs = mapTTs.Skip(_resultsPerPage * (page - 1)).Take(_resultsPerPage).ToList();
            var pageCount = Math.Ceiling((double) mapTTs.Count / _resultsPerPage);

            if (desiredWRs.Any())
            {
                var embedBuilder = new EmbedBuilder
                {
                    Title = "Recent Map TTs",
                    Description = string.Join(Environment.NewLine, desiredWRs.Select(x =>
                        $"{(x.RecordInfo.Class == 4 ? Config.DemomanEmoji : Config.SoldierEmoji)} #{x.RecordInfo.Rank.Value} • {Format.Url(Format.Bold(Format.Sanitize(x.PlayerInfo.RealName ?? x.PlayerInfo.Name)), TempusHubHelper.PlayerUrl(x.PlayerInfo.Id))} on {Format.Url(Format.Bold(Format.Sanitize(x.MapInfo.Name)), TempusHubHelper.MapUrl(x.MapInfo.Name))} • {Format.Bold(RecordUtilities.FormattedDuration(x.RecordInfo.Duration))} (WR +{RecordUtilities.FormattedDuration(x.RecordInfo.Duration - x.CachedTime.CurrentWRDuration.Value)}) • {x.RecordInfo.Date.GetPrettyTimeSince()}"))
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