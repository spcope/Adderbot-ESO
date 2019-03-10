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

    public class Adderbot
    {
        public static SocketRole trialLead = null;
        public static AdderData data;

        public async Task StartAsync()
        {
            IServiceCollection services = new ServiceCollection()
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

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetRequiredService<StartupService>().StartAsync();

            serviceProvider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }

        public static void Save()
        {
            TextWriter tw = new StreamWriter($@"{Directory.GetCurrentDirectory()}\adderbot.json");
            tw.Write(data.ToJson());
            tw.Close();
        }
    }
}
