using System;
using Adderbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;
using Adderbot.Models;

namespace Adderbot
{

    public static class Adderbot
    {
        public static bool InDebug = false;
        public static AdderData Data;
        public static ulong BotId = 376194715726512138;
        public static ulong DevId = 110881914956623872;
        public const string LiveToken = "Adderbot";
        public const string TestToken = "AdderbotTest";
        public const string LiveFile = @"C:\Adderbot\adderbot.json";
        public const string TestFile = @"C:\Adderbot\adderbot-test.json";
        public const bool IsLive = false;

        public static ulong[] BannedGuilds = new ulong[]
        {
            412466947683385344,
            548657326018527200
        };
        
        public static string[] EmoteNames = new string[]
        {
            "denhealer", "dkhealer", "nbhealer", "plarhealer", "sorchealer", "necrohealer",
            "dentank", "dktank", "nbtank", "plartank", "sorctank", "necrotank",
            "magblade", "magden", "magdk", "magplar", "magsorc", "magcro",
            "stamblade", "stamden", "stamdk", "stamplar", "stamsorc", "stamcro"
        };

        public static Random randomGen;

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
            
            randomGen = new Random();

            await serviceProvider.GetRequiredService<StartupService>().StartAsync();

            serviceProvider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }

        public static void Save()
        {
            File.WriteAllLines(IsLive ? LiveFile : TestFile, new [] { Data.ToJson() });
        }
    }
}
