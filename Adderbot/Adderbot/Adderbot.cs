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
        public static string[] EmoteNames = new string[]
        {
            "denhealer", "dkhealer", "nbhealer", "plarhealer", "sorchealer", "necrohealer",
            "dentank", "dktank", "nbtank", "plartank", "sorctank", "necrotank",
            "magblade", "magden", "magdk", "magplar", "magsorc", "magcro",
            "stamblade", "stamden", "stamdk", "stamplar", "stamsorc", "stamcro"
        };

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
            File.WriteAllLines(@"C:\Adderbot\adderbot.json", new [] { Data.ToJson() });
        }
    }
}
