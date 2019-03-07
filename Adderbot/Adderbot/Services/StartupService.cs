using Adderbot.Modules.Roles;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Adderbot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandSvc;
        
        public StartupService(DiscordSocketClient client, CommandService commandSvc)
        {
            _client = client;
            _commandSvc = commandSvc;
        }

        public async Task StartAsync()
        {
            string discordToken = Environment.GetEnvironmentVariable("Adderbot");
            if(string.IsNullOrWhiteSpace(discordToken))
            {
                throw new Exception("Token is empty. Please make sure to add it to your environment levels.");
            }

            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            await _commandSvc.AddModuleAsync<BaseModule>(null);
            await _commandSvc.AddModuleAsync<TankModule>(null);
            await _commandSvc.AddModuleAsync<HealerModule>(null);
            await _commandSvc.AddModuleAsync<MeleeModule>(null);
            await _commandSvc.AddModuleAsync<RangedModule>(null);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
