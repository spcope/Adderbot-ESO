using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.BadArgCount)
                {
                    await context.User.SendMessageAsync(
                        "Not enough arguments for that command were provided. Use the help command to check out the commands and their arguments.");
                } 
                else if(result.Error != CommandError.UnknownCommand)
                {
                    await context.User.SendMessageAsync(result.ErrorReason);
                    await context.Message.DeleteAsync();
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
