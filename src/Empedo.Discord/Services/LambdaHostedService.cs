using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Empedo.Discord.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Empedo.Discord.Services
{
    public class LambdaHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<LambdaHostedService> _logger;
        private readonly TempusEmbedService _tempusEmbedService;
        private readonly IConfiguration _configuration;
        private readonly DiscordClient _discordClient;
        private Timer _timer;

        public LambdaHostedService(ILogger<LambdaHostedService> logger, TempusEmbedService tempusEmbedService, IConfiguration configuration, DiscordClient discordClient)
        {
            _logger = logger;
            _tempusEmbedService = tempusEmbedService;
            _configuration = configuration;
            _discordClient = discordClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TickAsync, null, TimeSpan.Zero, 
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        
        private async void TickAsync(object state)
        {
            var tasks = new List<Task>
            {
                UpdateOverviewsAsync()
            };

            await Task.WhenAll(tasks);
        }

        private async Task WipeChannelAsync(DiscordChannel discordChannel)
        {
            var messages = await discordChannel.GetMessagesAsync();

            if (messages == null || !messages.Any())
            {
                return;
            }
            
            await discordChannel.DeleteMessagesAsync(messages);
        }

        private async Task<DiscordChannel> GetAndWipeChannelAsync(string configurationPath)
        {
            var channel = await _discordClient.GetChannelAsync(ulong.Parse(_configuration[configurationPath]));

            await WipeChannelAsync(channel);

            return channel;
        }

        private async Task UpdateOverviewsAsync()
        {
            var channel = await GetAndWipeChannelAsync("Lambda:OverviewsChannelId");
            
            var serverOverviewEmbeds = await _tempusEmbedService.GetServerOverviewAsync();
            await serverOverviewEmbeds.SendAll(channel);

            var topPlayerOnlineEmbeds = await _tempusEmbedService.GetTopPlayersOnlineAsync();
            await topPlayerOnlineEmbeds.SendAll(channel);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}