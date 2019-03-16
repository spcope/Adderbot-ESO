using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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
        public async Task SetLeadRole(string userRole)
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
            }
            else
                await Context.User.SendMessageAsync($"Unable to set trial lead role to {userRole} in "
                    + $"{Context.Guild.Name}/{Context.Channel.Name}");
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
                                else
                                {
                                    if (!r.AllowedRoles.Contains(sr.Id))
                                    {
                                        r.AllowedRoles.Add(sr.Id);
                                        await Context.User.SendMessageAsync($"Added {userRole} to the list of allowed roles for the raid.");
                                        await ((IUserMessage)raidMessage)
                                            .ModifyAsync(x =>
                                            {
                                                x.Embed = r.BuildEmbed();
                                                x.Content = r.BuildAllowedRoles();
                                            });
                                    }
                                    else
                                    {
                                        await Context.User.SendMessageAsync($"{userRole} was already in the list and couldn't be added.");
                                    }
                                    if (r.AllowedRoles.Contains(0))
                                    {
                                        r.AllowedRoles.Remove(0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("removeallowedrole")]
        [Summary("Removes a user role from the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RemoveAllowedRoleAsync([Remainder] string userRole)
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
                                r.AllowedRoles.Remove(sr.Id);
                                if (r.AllowedRoles.Count() == 0)
                                    r.AllowedRoles.Add(0);
                                await Context.User.SendMessageAsync($"Removed {userRole} from the list of allowed roles for the raid.");
                                await ((IUserMessage)raidMessage)
                                    .ModifyAsync(x =>
                                    {
                                        x.Embed = r.BuildEmbed();
                                        x.Content = r.BuildAllowedRoles();
                                    });
                            }
                        }
                    }
                }
            }
        }

        [Command("create")]
        [Summary("Creates a raid.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CreateAsync(string rclass, string rtype, string date, string time, string timezone, int mNum = 0, int rNum = 0, 
            int fNum = 8, [Remainder] string progRole = null)
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
                        try
                        {
                            var jr = new AdderRaid(rclass, rtype.ToLower(), date, time, timezone, Context.User.Id, mNum, rNum, fNum, ars);
                            jr.MessageId = (await ReplyAsync(jr.BuildAllowedRoles(), false, jr.BuildEmbed())).Id;
                            g.ActiveRaids.Add(new AdderChannel(Context.Channel.Id, jr));
                        }
                        catch (ArgumentException ae)
                        {
                            await Context.User.SendMessageAsync(ae.Message);
                        }
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("You do not have permission to create raids!");
                    }
                }
            }
        }

        [Command("remove")]
        [Summary("Removes the user from the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RemoveAsync()
        {
            ulong gid = Context.Guild.Id;
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == gid);
            if (g == null)
            {
                await Context.User.SendMessageAsync("The guild did not get added somehow. Contact the developer for help.");
            }
            else
            {
                AdderChannel ac = g.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (ac == null)
                {
                    await Context.User.SendMessageAsync($"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name}");
                }
                else
                {
                    AdderPlayer ap = ac.Raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == Context.User.Id);
                    if (ap != null)
                    {
                        ac.Raid.CurrentPlayers.Remove(ap);
                        await ((IUserMessage)await Context.Channel.GetMessageAsync(ac.Raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = ac.Raid.BuildEmbed();
                                x.Content = ac.Raid.BuildAllowedRoles();
                            });
                    }
                }
            }
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
                        await Context.User.SendMessageAsync("Deleted raid successfully!");
                    }
                }
            }
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
            }
        }

        [Command("raid-remove")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidRemoveAsync(string user)
        {
            ulong parsedUser = ulong.Parse(user.ToLower().Trim().Substring(2, user.Length - 3));
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (g == null)
            {
                await Context.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help.");
            }
            else
            {
                var ac = g.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (ac == null)
                {
                    await Context.User.SendMessageAsync("There is no raid in the channel to delete from.");
                }
                else
                {
                    if (ac.Raid.Lead == Context.User.Id)
                    {
                        var players = ac.Raid.CurrentPlayers.Where(x => x.PlayerId == parsedUser);
                        var users = Context.Guild.Users.Where(x => x.Id == parsedUser);
                        if (players.Count() == 0 || users.Count() == 0)
                        {
                            await Context.User.SendMessageAsync($"The raid in the channel does not have <@{parsedUser}> in it");
                        }
                        else
                        {
                            ac.Raid.CurrentPlayers.Remove(players.First());
                            await Context.Guild.GetUser(parsedUser).SendMessageAsync($"You have been removed from the raid in {Context.Channel.Name}.");
                            await ((IUserMessage)await Context.Channel.GetMessageAsync(ac.Raid.MessageId))
                                .ModifyAsync(x =>
                                {
                                    x.Embed = ac.Raid.BuildEmbed();
                                    x.Content = ac.Raid.BuildAllowedRoles();
                                });
                        }
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("You are not allowed to remove players from the raid!");
                    }
                }
            }
        }

        [Command("raid-add")]
        [Summary("Adds user as role (override)")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidAddAsync(string user, string role)
        {
            ulong parsedUser = ulong.Parse(user.Trim().Substring(2, user.Length - 3));

            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (g == null)
            {
                await Context.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help.");
            }
            else
            {
                var ac = g.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);

                if (ac == null)
                {
                    await Context.User.SendMessageAsync("There is no raid in the channel to delete from.");
                }
                else
                {
                    if (ac.Raid.Lead == Context.User.Id)
                    {
                        var players = Context.Guild.Users.Where(x => x.Id == parsedUser);
                        if (players.Count() == 0)
                        {
                            await Context.User.SendMessageAsync($"The user {user} doesn't exist in the channel.");
                        }
                        else
                        {
                            AdderPlayer pl = ac.Raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == parsedUser);
                            if (pl == null)
                            {
                                Role r = Role.InvalidRole;
                                switch (role)
                                {
                                    #region Tank
                                    case "t":
                                        r = Role.T;
                                        break;
                                    case "mt":
                                        r = Role.MT;
                                        break;
                                    case "ot":
                                        r = Role.OT;
                                        break;
                                    case "ot2":
                                        r = Role.OT2;
                                        break;
                                    #endregion

                                    #region Healer
                                    case "h":
                                        r = Role.H;
                                        break;
                                    case "h1":
                                        r = Role.H1;
                                        break;
                                    case "h2":
                                        r = Role.H2;
                                        break;
                                    case "kh":
                                        r = Role.KH;
                                        break;
                                    case "gh":
                                        r = Role.GH;
                                        break;
                                    #endregion

                                    #region Melee
                                    case "m":
                                        r = Role.mDPS;
                                        break;
                                    case "m1":
                                        r = Role.mDPS1;
                                        break;
                                    case "m2":
                                        r = Role.mDPS2;
                                        break;
                                    case "m3":
                                        r = Role.mDPS3;
                                        break;
                                    case "m4":
                                        r = Role.mDPS4;
                                        break;
                                    #endregion

                                    #region Ranged
                                    case "r":
                                        r = Role.rDPS;
                                        break;
                                    case "r1":
                                        r = Role.rDPS1;
                                        break;
                                    case "r2":
                                        r = Role.rDPS2;
                                        break;
                                    case "r3":
                                        r = Role.rDPS3;
                                        break;
                                    case "r4":
                                        r = Role.rDPS4;
                                        break;
                                        #endregion
                                }
                                ac.Raid.CurrentPlayers.Add(new AdderPlayer(players.First().Id, r));
                                await Context.Guild.GetUser(parsedUser).SendMessageAsync($"You have been added to the raid in {Context.Channel.Name}.");
                                await Context.User.SendMessageAsync($"Added {user} to the raid in {Context.Channel.Name}");
                                await ((IUserMessage)(await Context.Channel.GetMessageAsync(ac.Raid.MessageId)))
                                    .ModifyAsync(x =>
                                    {
                                        x.Embed = ac.Raid.BuildEmbed();
                                        x.Content = ac.Raid.BuildAllowedRoles();
                                    });
                            }
                            else
                            {
                                await Context.User.SendMessageAsync($"{user} was not on the raid roster and could not be removed.");
                            }
                        }
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("You are not allowed to forcefully add players to the raid!");
                    }
                }
            }
        }

        [Command("help")]
        [Summary("Sends the help message")]
        public async Task HelpAsync()
        {
            bool isAdmin = false;
            bool isTrialLead = false;
            AdderGuild g = Adderbot.data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (g == null || ((SocketGuildUser)Context.User).GuildPermissions.Administrator)
            {
                isAdmin = true;
                isTrialLead = true;
            }
            else
            {
                AdderChannel ac = g.ActiveRaids.FirstOrDefault(x => x.Raid.Lead == Context.User.Id);
                if (ac != null || ((SocketGuildUser)Context.User).Roles.Where(x => x.Id == g.Lead).Count() != 0)
                {
                    isTrialLead = true;
                }
            }
            var eb = new EmbedBuilder();
            if (isAdmin)
            {
                eb.Color = Color.Green;
                eb.Description = CommandHelp.AdminCommandHelp.ToString();
                await Context.User.SendMessageAsync(null, false, eb.Build());
            }
            if (isTrialLead)
            {
                eb.Color = Color.Purple;
                eb.Description = CommandHelp.RaidLeadCommandHelp.ToString();
                await Context.User.SendMessageAsync(null, false, eb.Build());
            }
            eb.Color = Color.Blue;
            eb.Description = CommandHelp.BasicCommandHelp.ToString();
            await Context.User.SendMessageAsync(null, false, eb.Build());
        }

        public static async Task UpdateRoster(SocketCommandContext scc, Role role, string user, bool or = false)
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
                    if (!ar.Raid.AllowedRoles.Contains(0) && !or
                        && ((SocketGuildUser)scc.User).Roles.FirstOrDefault(x => ar.Raid.AllowedRoles.Contains(x.Id)) == null)
                    {
                        await scc.User.SendMessageAsync($"You aren't allowed to join this raid, please message <@{ar.Raid.Lead}> for more information.");
                    }
                    else if (ar.Raid.CurrentPlayers.Where(x => x.PlayerId == scc.User.Id).Count() != 0)
                    {
                        await scc.User.SendMessageAsync($"You already joined this raid, if you want to change your role, remove" +
                            $" yourself from it and add yourself again.");
                    }
                    else
                    {
                        ar.Raid.AddPlayer(scc.User.Id, role);
                        await ((IUserMessage)await scc.Channel.GetMessageAsync(ar.Raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = ar.Raid.BuildEmbed();
                                x.Content = ar.Raid.BuildAllowedRoles();
                            });
                    }
                }
            }
        }

    }
}