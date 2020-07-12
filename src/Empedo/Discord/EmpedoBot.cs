using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Empedo.Models;
using Empedo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Empedo.Discord
{
    public class EmpedoBot
    {
        private readonly string _discordBotToken;
        private readonly DiscordSocketClient _discordClient;
        private readonly EmpedoCommandService _empedoCommandService;

        public EmpedoBot(string discordBotToken, Config config)
        {
            _discordBotToken = discordBotToken;

            var discordClientConfig = new DiscordSocketConfig
            {
                // Download users so that all users are available in large guilds
                AlwaysDownloadUsers = true,
                // Keeps messages from channels in cache, per channel
                MessageCacheSize = 50
            };

            // Create a new discord bot client, with the config
            _discordClient = new DiscordSocketClient(discordClientConfig);

            var baseCommandService = EmpedoCommandService.BuildBaseCommandService();

            // Add services to dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton(_discordClient)
                .AddSingleton(baseCommandService)
                .AddSingleton(config)
                .BuildServiceProvider();

            _empedoCommandService =
                new EmpedoCommandService(_discordClient, baseCommandService, serviceProvider, config);
        }

        internal async Task<Exception> RunAsync()
        {
            try
            {
                // Register commands
                await _empedoCommandService.InitializeAsync();

                // Login and start bot
                await _discordClient.LoginAsync(TokenType.Bot, _discordBotToken);
                await _discordClient.StartAsync();

                // Block the task indefinitely
                await Task.Delay(-1);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}