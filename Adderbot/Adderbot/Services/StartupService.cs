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
            _client.GuildAvailable += ValidateGuilds;
            _client.JoinedGuild += ValidateGuilds;
            _client.ChannelDestroyed += ValidateChannels;
            LoadJSON();

            await _commandSvc.AddModuleAsync<BaseModule>(null);
            await _commandSvc.AddModuleAsync<TankModule>(null);
            await _commandSvc.AddModuleAsync<HealerModule>(null);
            await _commandSvc.AddModuleAsync<MeleeModule>(null);
            await _commandSvc.AddModuleAsync<RangedModule>(null);
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

        private Task ValidateGuilds(SocketGuild arg)
        {
            List<AdderGuild> ags = new List<AdderGuild>();
            foreach (var ag in Adderbot.data.Guilds)
            {
                bool ne = true;
                foreach (var cg in _client.Guilds)
                    if (cg.Id == ag.GuildId)
                        ne = false;
                if (ne)
                    ags.Add(ag);
            }

            foreach(var ag in ags)
            {
                Adderbot.data.Guilds.Remove(ag);
            }

            foreach (var cg in _client.Guilds)
            {
                bool ne = true;
                foreach (var ag in Adderbot.data.Guilds)
                    if (ag.GuildId == cg.Id)
                        ne = false;
                if (ne)
                    Adderbot.data.Guilds.Add(new AdderGuild(cg.Id, 0));
            }
            Adderbot.Save();
            return Task.CompletedTask;
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
                StreamReader sr = new StreamReader($@"{Directory.GetCurrentDirectory()}\adderbot.json");
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
