using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adderbot
{
    class BaseModule : ModuleBase<SocketCommandContext>
    {
        [Command("setleadrole")]
        [Summary("Sets the role to be used as trial lead")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task SetLeadRole([Remainder] string userRole = null)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
            if (role != null)
            {
                await Context.User.SendMessageAsync($"Successfully set trial lead role to {userRole} in "
                    + $"{Context.Guild.Name}/{Context.Channel.Name}");
                ulong gid = Context.Guild.Id;
                AdderGuild guild = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == gid);
                if (guild == null)
                    Adderbot.data.Guilds.Add(new AdderGuild(gid, role.Id));
                else
                    guild.Lead = role.Id;
                Adderbot.Save();
            }
            else
                await Context.User.SendMessageAsync($"Unable to set trial lead role to {userRole} in "
                    + $"{Context.Guild.Name}/{Context.Channel.Name}");
            await Context.Message.DeleteAsync();
        }

        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
        {
            ulong gid = Context.Guild.Id;
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == gid);
            if (g == null)
            {
                await Context.User.SendMessageAsync($"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
            }
            else
            {
                AdderChannel ar = g.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (ar == null)
                {
                    await Context.User.SendMessageAsync($"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                }
                else
                {
                    AdderRaid r = ar.Raid;

                    if (r == null)
                    {
                        await Context.User.SendMessageAsync($"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                    }
                    else
                    {

                        if (r.Lead != Context.User.Id)
                        {
                            await Context.User.SendMessageAsync("You are not the lead for this raid and cannot add allowed roles!");
                        }
                        else
                        {
                            SocketRole sr = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
                            if (sr == null)
                            {
                                await Context.User.SendMessageAsync("Please enter a valid role!");
                            }
                            else
                            {
                                var raidMessage = await Context.Channel.GetMessageAsync(r.MessageId);
                                if (raidMessage == null)
                                {
                                    await Context.User.SendMessageAsync($"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                                }
                                r.AllowedRoles.Add(sr.Id);
                                r.AllowedRoles.Remove(0);
                                Adderbot.Save();
                                await Context.User.SendMessageAsync($"Added {userRole} to the list of allowed roles for the raid.");
                                await ((IUserMessage)raidMessage).ModifyAsync(x => x.Content = r.Build());
                            }
                        }
                    }
                }
            }
            await Context.Message.DeleteAsync();
        }

        [Command("create")]
        [Summary("Used for bad parse.")]
        public async Task CreateBadAsync([Remainder] string args)
        {
            await ReplyAsync("Invalid Command Usage! Syntax:\n`;create <trial difficulty> <trial acronym> " +
                "<date> <time> <timezone> <disallow melee (optional)> <allowed roles separated by | (optional>`");
        }

        [Command("create")]
        [Summary("Creates a raid.")]
        public async Task CreateAsync(string rclass, string rtype, string date, string time, string timezone, bool noMelee = false, [Remainder] string progRole = null)
        {
            ulong gid = Context.Guild.Id;
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == gid);
            if (g == null)
            {
                await Context.User.SendMessageAsync("The guild did not get added somehow. Contact the developer for help.");
            }
            else
            {
                if (g.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id) != null)
                {
                    await Context.User.SendMessageAsync($"There is already a raid in {Context.Guild.Name}/{Context.Channel.Name}");
                }
                else
                {
                    if (g.Lead == 0 || ((SocketGuildUser)Context.User).Roles.FirstOrDefault(x => x.Id == g.Lead) != null
                        || ((SocketGuildUser)Context.User).GuildPermissions.Administrator)
                    {
                        List<ulong> ars = new List<ulong>();
                        if (progRole == null)
                        {
                            ars.Add(0);
                        }
                        else
                        {
                            var progRoles = progRole.Split('|');
                            foreach (var pr in progRoles)
                            {
                                var tr = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(pr.Trim()));
                                if (tr == null)
                                {
                                    await Context.User.SendMessageAsync($"Could not add {tr.Name} as an allowed role.");
                                }
                                else
                                {
                                    if (!ars.Contains(tr.Id))
                                    {
                                        ars.Add(tr.Id);
                                    }
                                }
                            }
                        }
                        AdderRaid jr = new AdderRaid(rclass, rtype, date, time, timezone, Context.User.Id, noMelee, ars);
                        jr.MessageId = (await ReplyAsync(jr.Build())).Id;
                        g.ActiveRaids.Add(new AdderChannel(Context.Channel.Id, jr));
                        Adderbot.Save();
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("You do not have permission to create raids!");
                    }
                }
            }
            await Context.Message.DeleteAsync();
        }

        [Command("delete")]
        [Summary("Deletes raid associated with user.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteAsync()
        {
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (g == null)
            {
                await Context.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help!");
            }
            else
            {
                var ar = g.ActiveRaids.FirstOrDefault(x => (x.ChannelId == Context.Channel.Id && x.Raid?.Lead == Context.User.Id));

                if (ar == null)
                {
                    await Context.User.SendMessageAsync("There are either no raids, or no raids led by you in the channel!");
                }
                else
                {
                    var m = await Context.Channel.GetMessageAsync(ar.Raid.MessageId);
                    if (m == null)
                    {
                        await Context.User.SendMessageAsync("The raid to be deleted could not be found!");
                    }
                    else
                    {
                        await m.DeleteAsync();
                        g.ActiveRaids.Remove(ar);
                        Adderbot.Save();
                        await Context.User.SendMessageAsync("Deleted raid successfully!");
                    }
                }
            }
            await Context.Message.DeleteAsync();
        }

        [Command("purge")]
        [Summary("Purges channel of all messages")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeAsync()
        {
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (g == null)
            {
                await Context.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help.");
            }
            else
            {
                var ar = g.ActiveRaids.FirstOrDefault(x => (x.ChannelId == Context.Channel.Id));

                if (ar != null)
                {
                    g.ActiveRaids.Remove(ar);
                }
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(await Context.Channel.GetMessagesAsync(100).FlattenAsync());
                IUserMessage m = await ReplyAsync($"Purge completed. _This message will be deleted in 5 seconds_");
                for (int i = 4; i > 0; i--)
                {
                    await Task.Delay(1000);
                    await m.ModifyAsync(x => x.Content = $"Purge completed. _This message will be deleted in {i} seconds_");
                }
                await m.DeleteAsync();
                Adderbot.Save();
            }
        }

        public static async Task UpdateRoster(SocketCommandContext scc, Role role, string user)
        {
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == scc.Guild.Id);
            if (g == null)
            {
                await scc.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help.");
            }
            else
            {
                AdderChannel ar = g.ActiveRaids.FirstOrDefault(x => x.ChannelId == scc.Channel.Id);
                if (ar == null)
                {
                    await scc.User.SendMessageAsync("There is not a raid in this channel!");
                }
                else
                {
                    if (!ar.Raid.AllowedRoles.Contains(0)
                        && ((SocketGuildUser)scc.User).Roles.FirstOrDefault(x => ar.Raid.AllowedRoles.Contains(x.Id)) == null)
                    {
                        await scc.User.SendMessageAsync($"You aren't allowed to join this raid, please message <@{ar.Raid.Lead}> for more information.");
                    }
                    else
                    {
                        ar.Raid.AddPlayer(scc.User.Id, role);
                        Adderbot.Save();
                        await ((IUserMessage)await scc.Channel.GetMessageAsync(ar.Raid.MessageId)).ModifyAsync(x => x.Content = ar.Raid.Build());
                    }
                }
            }
            await scc.Channel.DeleteMessageAsync(scc.Message);
        }
    }
}