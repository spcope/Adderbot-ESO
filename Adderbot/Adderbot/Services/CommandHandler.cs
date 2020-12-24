using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Adderbot.Constants;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adderbot.Services
{
    internal class CommandHandler
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(IServiceProvider provider)
        {
            _config = provider.GetRequiredService<IConfiguration>();
            _client = provider.GetService<DiscordSocketClient>();
            _commands = provider.GetService<CommandService>();
            if (_client == null || _commands == null)
                throw new HttpRequestException("Could not connect to Discord! Exiting....");
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += CommandExecuteAsync;
            _provider = provider;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message) || message.Source != MessageSource.User) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) ||
                  message.HasCharPrefix(';', ref argPos))) return;
            
            var context = new SocketCommandContext(_client, message);
            
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _provider);
        }
        
        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task CommandExecuteAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // If the command could not execute send appropriate error
            if (!result.IsSuccess)
            {
                // If not enough arguments send error message
                if (result.Error == CommandError.BadArgCount)
                {
                    await context.User.SendMessageAsync(MessageText.Error.CommandBadArgCount);
                }
                else if (result.Error != CommandError.UnknownCommand)
                {
                    await context.User.SendMessageAsync(MessageText.Error.CommandInvalid);
                }
            }
            else
            {
                await context.Message.DeleteAsync();
                Adderbot.Save();
            }
            
            await Task.CompletedTask;
        }
    }
}