using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Adderbot.Constants;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Adderbot.Services
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _commands = _provider.GetService<CommandService>();
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message)) return;

            // Create a number to track where the prefix ends and the command begins
            var argPos = 0;
            
            var context = new SocketCommandContext(_client, message);
            
            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argPos) || message.HasCharPrefix(';', ref argPos))) return;

            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _provider);

            // If the command could not execute send appropriate error
            if (!result.IsSuccess)
            {
                // If not enough arguments send error message
                if (result.Error == CommandError.BadArgCount)
                {
                    await context.User.SendMessageAsync(MessageText.Error.CommandBadArgCount);
                } 
                else if(result.Error != CommandError.UnknownCommand)
                {
                    await context.User.SendMessageAsync(MessageText.Error.CommandInvalid);
                }
            }
            else
            {
                await context.Message.DeleteAsync();
                Adderbot.Save();
            }
        }
    }
}
