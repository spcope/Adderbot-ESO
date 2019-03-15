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
            LoadJSON();

            await _commandSvc.AddModuleAsync<BaseModule>(null);
            await _commandSvc.AddModuleAsync<TankModule>(null);
            await _commandSvc.AddModuleAsync<HealerModule>(null);
            await _commandSvc.AddModuleAsync<MeleeModule>(null);
            await _commandSvc.AddModuleAsync<RangedModule>(null);
        }

        private Task RemoveGuild(SocketGuild sg)
        {
            AdderGuild ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == sg.Id);
            if(ag != null)
            {
                Adderbot.data.Guilds.Remove(ag);
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private Task ValidateGuilds(Cacheable<IMessage, ulong> m, ISocketMessageChannel c)
        {
            AdderGuild ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == ((IGuildChannel)c).GuildId);
            if(ag != null)
            {
                var am = ag.ActiveRaids.FirstOrDefault(x => x.Raid.MessageId == m.Id);
                if(am != null)
                {
                    ag.ActiveRaids.Remove(am);
                }
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private Task ValidateChannels(SocketChannel sg)
        {
            AdderGuild ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == ((SocketGuildChannel) sg).Guild.Id);
            if (ag != null)
            {
                List<AdderChannel> tbd = new List<AdderChannel>();
                foreach (var a in ag.ActiveRaids)
                    if (sg.Id == a.ChannelId)
                        tbd.Add(a);
                foreach (var d in tbd)
                    ag.ActiveRaids.Remove(d);
            }
            Adderbot.Save();
            return Task.CompletedTask;
        }

        private async Task ValidateGuilds(SocketGuild sg)
        {
            AdderGuild ag = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == sg.Id);
            if (ag == null)
            {
                Adderbot.data.Guilds.Add(new AdderGuild(sg.Id, 0));
            }
            else
            {
                List<AdderChannel> tbd = new List<AdderChannel>();
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

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private void LoadJSON()
        {
            try
            {
                StreamReader sr = new StreamReader($@"C:\Adderbot\adderbot.json");
                if (!sr.EndOfStream)
                    Adderbot.data = AdderData.FromJson(sr.ReadToEnd());
                else
                    Adderbot.data = new AdderData();
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