using Adderbot.Models;
using Adderbot.Modules.Roles;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Modules.Base;

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
            var discordToken = Environment.GetEnvironmentVariable("Adderbot");
            if(string.IsNullOrWhiteSpace(discordToken))
            {
                throw new Exception("Token is empty. Please make sure to add it to your environment vars.");
            }

            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();
            await _client.SetGameAsync(";help for a list of commands");
            _client.GuildAvailable += ValidateGuilds;
            _client.JoinedGuild += ValidateGuilds;
            _client.ChannelDestroyed += ValidateChannels;
            _client.LeftGuild += RemoveGuild;
            _client.MessageDeleted += ValidateGuilds;
            LoadJson();

            await _commandSvc.AddModuleAsync<BaseModule>(null);
            await _commandSvc.AddModuleAsync<TankModule>(null);
            await _commandSvc.AddModuleAsync<HealerModule>(null);
            await _commandSvc.AddModuleAsync<MeleeModule>(null);
            await _commandSvc.AddModuleAsync<RangedModule>(null);
        }

        private static Task RemoveGuild(SocketGuild sg)
        {
            var ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == sg.Id);
            if(ag != null)
            {
                Adderbot.data.Guilds.Remove(ag);
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private static Task ValidateGuilds(Cacheable<IMessage, ulong> m, ISocketMessageChannel c)
        {
            var ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == ((IGuildChannel)c).GuildId);
            var am = ag?.ActiveRaids.FirstOrDefault(x => x.Raid.MessageId == m.Id);
            if(am != null)
            {
                ag.ActiveRaids.Remove(am);
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private static Task ValidateChannels(SocketChannel sg)
        {
            var ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == ((SocketGuildChannel) sg).Guild.Id);
            if (ag != null)
            {
                var tbd = new List<AdderChannel>();
                foreach (var a in ag.ActiveRaids)
                    if (sg.Id == a.ChannelId)
                        tbd.Add(a);
                foreach (var d in tbd)
                    ag.ActiveRaids.Remove(d);
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private static async Task ValidateGuilds(SocketGuild sg)
        {
            var ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == sg.Id);
            if (ag == null)
            {
                Adderbot.data.Guilds.Add(new AdderGuild(sg.Id, 0));
            }
            else
            {
                var tbd = new List<AdderChannel>();
                foreach(var g in ag.ActiveRaids)
                {
                    var c = sg.Channels.FirstOrDefault(x => x.Id == g.ChannelId);
                    if(c == null)
                    {
                        tbd.Add(g);
                    }
                    else
                    {
                        var m = await ((ISocketMessageChannel)c).GetMessageAsync(g.Raid.MessageId);
                        if(m == null)
                        {
                            tbd.Add(g);
                        }
                    }
                }
                foreach(var d in tbd)
                {
                    ag.ActiveRaids.Remove(d);
                }
            }
            Adderbot.Save();
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private static void LoadJson()
        {
            try
            {
                var reader = new StreamReader(@"C:\Adderbot\adderbot.json");
                Adderbot.data = !reader.EndOfStream ? AdderData.FromJson(reader.ReadToEnd()) : new AdderData();
            }
            catch (FileNotFoundException)
            {
                Adderbot.data = new AdderData();
            }
            catch (JsonException je)
            {
                Console.WriteLine(je.StackTrace);
            }
        }
    }
}