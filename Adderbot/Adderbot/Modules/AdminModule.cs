using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    internal class AdminModule : BaseModule
    {
        [Command("summon")]
        [Summary("@s the raiders listed in the raid in the channel")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SummonRaiders()
        {
            var raid = GetRaid().Result;
            if (raid != null)
            {
                await ReplyAsync(
                    $"{raid.BuildPlayers()}{raid.Type} led by <@{raid.Lead}> is forming. X up in guild or whisper them in game.");
            }
        }

        [Command("purge")]
        [Summary("Purges channel of all messages")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeAsync()
        {
            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "Somehow the guild did not get added. Contact the developer for help.");
            }

            else
            {
                var ar = guild.ActiveRaids.FirstOrDefault(x => (x.ChannelId == Context.Channel.Id));

                if (ar != null)
                {
                    guild.ActiveRaids.Remove(ar);
                }

                await ((ITextChannel) Context.Channel).DeleteMessagesAsync(await Context.Channel.GetMessagesAsync()
                    .FlattenAsync());
                var purgeMessage = await ReplyAsync("Purge completed. _This message will be deleted in 5 seconds_");
                for (var i = 4; i > 0; i--)
                {
                    await Task.Delay(1000);
                    await purgeMessage.ModifyAsync(x =>
                        x.Content = $"Purge completed. _This message will be deleted in {i} seconds_");
                }

                await purgeMessage.DeleteAsync();
            }
        }

        [Command("set-debug")]
        [Summary("Flips the debug switch, can turn on and off")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetDebug()
        {
            if (Context.User.Id == Adderbot.DevId)
            {
                Adderbot.InDebug = !Adderbot.InDebug;
                await Context.User.SendMessageAsync($"Set bot debug to {Adderbot.InDebug}");
            }
            else
            {
                await Context.User.SendMessageAsync(
                    "How you know this command exists, I don't know. But you can't use it.");
            }
        }

        [Command("setleadrole")]
        [Summary("Sets the role to be used as trial lead")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task SetLeadRole(string userRole)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
            if (role != null)
            {
                await Context.User.SendMessageAsync($"Successfully set trial lead role to {userRole} in "
                                                    + $"{Context.Guild.Name}/{Context.Channel.Name}");

                var gid = Context.Guild.Id;
                var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == gid);

                if (guild == null)
                    Adderbot.Data.Guilds.Add(new AdderGuild(gid, role.Id));
                else
                    guild.Lead = role.Id;
            }

            else
                await Context.User.SendMessageAsync($"Unable to set trial lead role to {userRole} in "
                                                    + $"{Context.Guild.Name}/{Context.Channel.Name}");
        }
    }
}