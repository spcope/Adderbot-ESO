using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Adderbot.Modules
{
    internal class RaidLeadModule : BaseModule
    {
        [Command("create")]
        [Summary("Creates a raid.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CreateAsync(string raidClass, string raidType, string date, string time, string timezone,
            int mNum =
                0, int rNum = 0,
            int fNum = -1, [Remainder] string progRole = null)
        {
            var guildId = Context.Guild.Id;

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == guildId);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "The guild did not get added somehow. Contact the developer for help.");
            }

            else
            {
                if (guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id) != null)
                {
                    await Context.User.SendMessageAsync(
                        $"There is already a raid in {Context.Guild.Name}/{Context.Channel.Name}");
                }
                else
                {
                    if (guild.Lead == 0 || ((SocketGuildUser) Context.User).Roles.FirstOrDefault(
                                            x => x.Id == guild.Lead) != null
                                        || ((SocketGuildUser) Context.User).GuildPermissions.Administrator)
                    {
                        var allowedRoles = new List<ulong>();
                        if (progRole == null)
                        {
                            allowedRoles.Add(0);
                        }
                        else
                        {
                            var progRoles = progRole.Split('|');
                            foreach (var pr in progRoles)
                            {
                                var tr = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(pr.Trim()));
                                if (tr == null)
                                {
                                    await Context.User.SendMessageAsync($"Could not add {pr} as an allowed role.");
                                }
                                else
                                {
                                    if (!allowedRoles.Contains(tr.Id))
                                    {
                                        allowedRoles.Add(tr.Id);
                                    }
                                }
                            }
                        }

                        try
                        {
                            var newRaid = new AdderRaid(raidClass, raidType.ToLower(), date, time, timezone,
                                Context.User.Id, mNum,
                                rNum, fNum, allowedRoles);
                            newRaid.MessageId = (await ReplyAsync(newRaid.BuildAllowedRoles(), false, newRaid.BuildEmbed())).Id;
                            guild.ActiveRaids.Add(new AdderChannel(Context.Channel.Id, newRaid));
                            Adderbot.Save();
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

        [Command("delete")]
        [Summary("Deletes raid associated with user.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteAsync()
        {
            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "Somehow the guild did not get added. Contact the developer for help!");
            }

            else
            {
                var activeRaid =
                    guild.ActiveRaids.FirstOrDefault(x =>
                        (x.ChannelId == Context.Channel.Id && x.Raid?.Lead == Context.User.Id));

                if (activeRaid == null)
                {
                    await Context.User.SendMessageAsync(
                        "There are either no raids, or no raids led by you in the channel!");
                }
                else
                {
                    var raidMessage = await Context.Channel.GetMessageAsync(activeRaid.Raid.MessageId);
                    if (raidMessage == null)
                    {
                        await Context.User.SendMessageAsync("The raid to be deleted could not be found!");
                    }
                    else
                    {
                        await raidMessage.DeleteAsync();
                        guild.ActiveRaids.Remove(activeRaid);
                        await Context.User.SendMessageAsync("Deleted raid successfully!");
                    }
                }
            }
        }

        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
        {
            var raid = GetRaid().Result;
            if (raid != null && CheckUserIsLead(raid).Result)
            {
                var socketRole =
                    Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
                if (socketRole == null)
                {
                    await Context.User.SendMessageAsync("Please enter a valid role!");
                }
                else
                {
                    var raidMessage = GetMessageById(raid.MessageId).Result;
                    if (raidMessage != null)
                    {
                        if (!raid.AllowedRoles.Contains(socketRole.Id))
                        {
                            raid.AllowedRoles.Add(socketRole.Id);
                            await Context.User.SendMessageAsync(
                                $"Added {userRole} to the list of allowed roles for the raid.");
                            await ((IUserMessage) raidMessage)
                                .ModifyAsync(x =>
                                {
                                    x.Embed = raid.BuildEmbed();
                                    x.Content = raid.BuildAllowedRoles();
                                });
                        }
                        else
                        {
                            await Context.User.SendMessageAsync(
                                $"{userRole} was already in the list and couldn't be added.");
                        }

                        if (raid.AllowedRoles.Contains(0))
                        {
                            raid.AllowedRoles.Remove(0);
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
            var guildId = Context.Guild.Id;

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == guildId);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
            }

            else
            {
                var activeRaid = guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (activeRaid == null)
                {
                    await Context.User.SendMessageAsync(
                        $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                }
                else
                {
                    var raid = activeRaid.Raid;

                    if (raid == null)
                    {
                        await Context.User.SendMessageAsync(
                            $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                    }
                    else
                    {
                        if (raid.Lead != Context.User.Id)
                        {
                            await Context.User.SendMessageAsync(
                                "You are not the lead for this raid and cannot add allowed roles!");
                        }
                        else
                        {
                            var socketRole =
                                Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
                            if (socketRole == null)
                            {
                                await Context.User.SendMessageAsync("Please enter a valid role!");
                            }
                            else
                            {
                                var raidMessage = await Context.Channel.GetMessageAsync(raid.MessageId);
                                if (raidMessage == null)
                                {
                                    await Context.User.SendMessageAsync(
                                        $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                                    return;
                                }

                                raid.AllowedRoles.Remove(socketRole.Id);
                                if (!raid.AllowedRoles.Any())
                                    raid.AllowedRoles.Add(0);
                                await Context.User.SendMessageAsync(
                                    $"Removed {userRole} from the list of allowed roles for the raid.");
                                await ((IUserMessage) raidMessage).ModifyAsync(x =>
                                {
                                    x.Embed = raid.BuildEmbed();
                                    x.Content = raid.BuildAllowedRoles();
                                });
                            }
                        }
                    }
                }
            }
        }

        [Command("raid-add")]
        [Summary("Adds user as role (override)")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidAddAsync(string user, string role, [Remainder] string emote = null)
        {
            var parsedUser = ParseUser(user);

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "Somehow the guild did not get added. Contact the developer for help.");
            }

            else
            {
                var adderChannel = guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);

                if (adderChannel == null)
                {
                    await Context.User.SendMessageAsync("There is no raid in the channel to delete from.");
                }
                else
                {
                    if (adderChannel.Raid.Lead == Context.User.Id)
                    {
                        if (Context.Guild.Users.All(x => x.Id != parsedUser) && !Adderbot.InDebug)
                        {
                            await Context.User.SendMessageAsync($"The user {user} doesn't exist in the channel.");
                        }
                        else
                        {
                            if (adderChannel.Raid.CurrentPlayers.All(x => x.PlayerId != parsedUser))
                            {
                                Role parsedRole;
                                switch (role)
                                {
                                    #region Tank

                                    case "mt":
                                        parsedRole = Role.Mt;
                                        break;
                                    case "ot":
                                        parsedRole = Role.Ot;
                                        break;

                                    #endregion

                                    #region Healer

                                    case "h1":
                                        parsedRole = Role.H1;
                                        break;
                                    case "h2":
                                        parsedRole = Role.H2;
                                        break;
                                    case "kh":
                                        parsedRole = Role.Kh;
                                        break;
                                    case "gh":
                                        parsedRole = Role.Gh;
                                        break;
                                    case "ch":
                                        parsedRole = Role.Ch;
                                        break;
                                    case "th":
                                        parsedRole = Role.Th;
                                        break;

                                    #endregion

                                    #region Dps

                                    case "dps":
                                        parsedRole = Role.Dps;
                                        break;
                                    case "m":
                                        parsedRole = Role.MDps;
                                        break;
                                    case "r":
                                        parsedRole = Role.RDps;
                                        break;

                                    #endregion

                                    default:
                                        parsedRole = Role.InvalidRole;
                                        break;
                                }

                                try
                                {
                                    adderChannel.Raid.AddPlayer(parsedUser, parsedRole,
                                        CheckEmoteValid(emote, Context).Result);
                                    await ((IUserMessage) await Context.Channel.GetMessageAsync(adderChannel.Raid
                                            .MessageId))
                                        .ModifyAsync(x =>
                                        {
                                            x.Embed = adderChannel.Raid.BuildEmbed();
                                            x.Content = adderChannel.Raid.BuildAllowedRoles();
                                        });
                                }
                                catch (ArgumentException ae)
                                {
                                    await Context.User.SendMessageAsync(ae.Message);
                                    return;
                                }

                                await Context.Guild.GetUser(parsedUser)
                                    .SendMessageAsync($"You have been added to the raid in {Context.Channel.Name}.");
                                await Context.User.SendMessageAsync(
                                    $"Added {user} to the raid in {Context.Channel.Name}");
                                await ((IUserMessage) (await Context.Channel.GetMessageAsync(
                                        adderChannel.Raid.MessageId)))
                                    .ModifyAsync(x =>
                                    {
                                        x.Embed = adderChannel.Raid.BuildEmbed();
                                        x.Content = adderChannel.Raid.BuildAllowedRoles();
                                    });
                            }
                            else
                            {
                                await Context.User.SendMessageAsync(
                                    $"{user} was on the raid roster already and could not be added.");
                            }
                        }
                    }
                    else
                    {
                        await Context.User.SendMessageAsync(
                            "You are not allowed to forcefully add players to the raid!");
                    }
                }
            }
        }

        [Command("raid-remove")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidRemoveAsync(string user)
        {
            var parsedUser = ParseUser(user);

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "Somehow the guild did not get added. Contact the developer for help.");
            }

            else
            {
                var ac = guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (ac == null)
                {
                    await Context.User.SendMessageAsync("There is no raid in the channel to delete from.");
                }
                else
                {
                    if (ac.Raid.Lead == Context.User.Id)
                    {
                        var players = ac.Raid.CurrentPlayers.Where(x => x.PlayerId == parsedUser).ToArray();
                        if ((!players.Any() || Context.Guild.Users.All(x => x.Id != parsedUser)) && !Adderbot.InDebug)
                        {
                            await Context.User.SendMessageAsync(
                                $"The raid in the channel does not have <@{parsedUser}> in it");
                        }
                        else
                        {
                            ac.Raid.CurrentPlayers.Remove(players.First());
                            if (!Adderbot.InDebug)
                                await Context.Guild.GetUser(parsedUser)
                                    .SendMessageAsync(
                                        $"You have been removed from the raid in {Context.Channel.Name}.");
                        }

                        await ((IUserMessage) await Context.Channel.GetMessageAsync(ac.Raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = ac.Raid.BuildEmbed();
                                x.Content = ac.Raid.BuildAllowedRoles();
                            });
                    }
                    else
                    {
                        await Context.User.SendMessageAsync("You are not allowed to remove players from the raid!");
                    }
                }
            }
        }
    }
}