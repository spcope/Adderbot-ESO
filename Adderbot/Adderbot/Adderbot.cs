﻿using Adderbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using Adderbot.Models;
using System;

namespace Adderbot
{

    public static class Adderbot
    {
        public static SocketRole trialLead = null;
        public static AdderData data;

        public static async Task StartAsync()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose,
                    CaseSensitiveCommands = false,
                    ThrowOnError = false
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<StartupService>();

            var serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetRequiredService<StartupService>().StartAsync();

            serviceProvider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }

        public static void Save()
        {
            File.WriteAllLines(@"C:\Adderbot\adderbot.json", new [] { data.ToJson() });
        }
    }
}
