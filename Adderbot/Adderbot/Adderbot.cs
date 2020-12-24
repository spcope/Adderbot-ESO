using System;
using System.Collections.Generic;
using Adderbot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adderbot.Helpers;
using Adderbot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Adderbot
{
    public class Adderbot
    {
        public static bool InDebug = false;
        public static AdderData Data;
        public static ulong BotId = 376194715726512138;
        public static ulong DevId = 110881914956623872;
        public const string LiveToken = "Adderbot";
        public const string TestToken = "AdderbotTest";
        public const string LiveFile = @"C:\Adderbot\adderbot.json";
        public const string TestFile = @"C:\Adderbot\adderbot-test.json";
        public const bool IsLive = true;
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;

        private static readonly object AdderFileLock = new object();
        private static readonly object AdderDataLock = new object();

        public static ulong[] BannedGuilds =
        {
            412466947683385344,
            548657326018527200
        };
        
        public static string[] EmoteNames =
        {
            "denhealer", "dkhealer", "nbhealer", "plarhealer", "sorchealer", "necrohealer",
            "dentank", "dktank", "nbtank", "plartank", "sorctank", "necrotank",
            "magblade", "magden", "magdk", "magplar", "magsorc", "magcro",
            "stamblade", "stamden", "stamdk", "stamplar", "stamsorc", "stamcro"
        };

        public static Random RandomGen;

        public Adderbot()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json").Build();
        }

        public async Task StartAsync()
        {
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            _client = client;

            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;
            await _client.SetGameAsync(";help for commands");
            
            _client.GuildAvailable += ValidateGuilds;
            _client.JoinedGuild += ValidateGuilds;
            _client.ChannelDestroyed += ValidateChannels;
            _client.LeftGuild += RemoveGuild;
            _client.MessageDeleted += ValidateGuilds;
            _client.UserLeft += RemoveUser;
            
            LoadJson();

            RandomGen = new Random();
                
            await client.LoginAsync(TokenType.Bot, _config["Token"]);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            await Task.Delay(-1);
        }

        public static void Save()
        {
            lock (AdderFileLock)
            {
                using var f = new FileStream(IsLive ? LiveFile : TestFile, FileMode.Truncate, FileAccess.Write,
                    FileShare.Read);
                using var sw = new StreamWriter(f, Encoding.UTF8);
                foreach (var str in new[] {Data.ToJson()})
                {
                    sw.WriteLine(str);
                }
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000,
                    AlwaysDownloadUsers = true
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose,
                    CaseSensitiveCommands = false,
                    ThrowOnError = false
                }))
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        private static Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as: {_client.CurrentUser}");
            return Task.CompletedTask;
        }
        
         private static Task RemoveUser(SocketGuildUser user)
        {
            var raidsToRemoveUserFrom = GuildHelper.FindRaidsUserIsIn(user.Guild, user.Id);
            return Task.CompletedTask;
        }

        private static Task RemoveGuild(SocketGuild sg)
        {
            lock (AdderDataLock)
            {
                var ag = Data.Guilds.FirstOrDefault(x => x.GuildId == sg.Id);
                if (ag != null)
                {
                    Data.Guilds.Remove(ag);
                }
            }

            Save();
            return Task.CompletedTask;
        }

        private static Task ValidateGuilds(Cacheable<IMessage, ulong> m, ISocketMessageChannel c)
        {
            lock (AdderDataLock)
            {
                var ag = Data.Guilds.FirstOrDefault(x => x.GuildId == ((IGuildChannel) c).GuildId);
                var am = ag?.ActiveRaids.FirstOrDefault(x => x.Raid.MessageId == m.Id);
                if (am != null)
                {
                    ag.ActiveRaids.Remove(am);
                }
            }

            Save();
            return Task.CompletedTask;
        }

        private static Task ValidateChannels(SocketChannel sg)
        {
            lock (AdderDataLock)
            {
                var ag = Data.Guilds.FirstOrDefault(x => x.GuildId == ((SocketGuildChannel) sg).Guild.Id);
                if (ag != null)
                {
                    var tbd = ag.ActiveRaids.Where(a => sg.Id == a.ChannelId).ToList();
                    foreach (var d in tbd)
                        ag.ActiveRaids.Remove(d);
                }
            }

            Save();
            return Task.CompletedTask;
        }

        private async Task ValidateGuilds(SocketGuild sg)
        {
            if (BannedGuilds.Contains(sg.Id))
            {
                await sg.LeaveAsync();
            }

            var ag = GuildHelper.GetGuildById(sg.Id);

            if (ag == null)
            {
                var g = new AdderGuild(sg.Id, 0) {EmotesAvailable = GuildHelper.CheckEmoteAvailability(sg)};
                lock (AdderDataLock)
                {
                    Data.Guilds.Add(g);
                }
            }
            else
            {
                var tbd = new List<AdderChannel>();
                foreach (var g in ag.ActiveRaids)
                {
                    var c = sg.Channels.FirstOrDefault(x => x.Id == g.ChannelId);
                    if (c == null)
                    {
                        tbd.Add(g);
                    }
                    else
                    {
                        var m = await ((ISocketMessageChannel) c).GetMessageAsync(g.Raid.MessageId);
                        if (m == null)
                        {
                            tbd.Add(g);
                        }
                    }
                }

                foreach (var d in tbd)
                {
                    ag.ActiveRaids.Remove(d);
                }

                ag.EmotesAvailable = GuildHelper.CheckEmoteAvailability(sg);
            }

            Save();
        }
        
        private static void LoadJson()
        {
            try
            {
                var reader = new StreamReader(IsLive ? LiveFile : TestFile);
                lock (AdderDataLock)
                {
                    Data = !reader.EndOfStream ? AdderData.FromJson(reader.ReadToEnd()) : new AdderData();
                }
            }
            catch (FileNotFoundException)
            {
                lock (AdderDataLock)
                {
                    Data = new AdderData();
                }
            }
            catch (JsonException je)
            {
                Console.WriteLine(je.StackTrace);
            }
        }
    }
}
