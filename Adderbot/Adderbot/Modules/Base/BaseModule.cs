using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Adderbot.Modules.Base
{
    internal class BaseModule : ModuleBase<SocketCommandContext>
    {
        private async Task<AdderGuild> CheckGuildValid(ulong guildId)
        {
            var guild = GuildHelper.GetGuildById(guildId);
            if (guild != null) return guild;
            await Context.User.SendMessageAsync(MessageText.Error.InvalidGuild);
            return null;
        }

        private static async Task<Emote> CheckEmoteValid(string emote, SocketCommandContext scc)
        {
            if (emote == null) return null;
            if (!Emote.TryParse(emote, out var parsedEmote))
            {
                await scc.User.SendMessageAsync("Emote not available. You'll still be added to the raid, but without the emote");
            }

            if (scc.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name)).Any(guildEmote => guildEmote.Id == parsedEmote.Id))
            {
                return parsedEmote;
            }
            
            await scc.User.SendMessageAsync("Emote not available. You'll still be added to the raid, but without the emote");

            return parsedEmote;
        }

        [Command("check-emotes")]
        [Summary("Checks which emotes are available")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CheckEmotes()
        {
            var guild = CheckGuildValid(Context.Guild.Id).Result;
            if (guild != null && guild.EmotesAvailable)
            {
                await ReplyAsync(
                    $"Available Emotes\n{Context.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name)).Aggregate("", (current, emote) => current + $"{emote} - {emote.Name}\n")}");
            }
        }

        [Command("add-emotes")]
        [Summary("Attempts to add all the emotes the bot uses & enables them")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageEmojis)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddEmotes()
        {
            try
            {
                var guild = CheckGuildValid(Context.Guild.Id).Result;
                if (guild != null)
                {
                    var path = Directory.GetCurrentDirectory();

                    foreach (var emote in Adderbot.EmoteNames)
                        await Context.Guild.CreateEmoteAsync(emote,
                            new Image($@"{path}\Media\{emote}.png"));

                    guild.EmotesAvailable = true;
                }
            }
            catch (Exception)
            {
                await Context.User.SendMessageAsync(
                    "Could not add all the emotes, typically this means you do not have enough space. " +
                    "At least 24 slots are required to add all emotes.");
            }
        }

        [Command("remove-emotes")]
        [Summary("Attempts to add all the emotes the bot uses & enables them")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageEmojis)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveEmotes()
        {
            try
            {
                var guild = CheckGuildValid(Context.Guild.Id).Result;
                if (guild != null)
                {
                    foreach (var emote in Context.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name)))
                        await Context.Guild.DeleteEmoteAsync(emote);

                    guild.EmotesAvailable = false;
                }
            }
            catch (Exception)
            {
                await Context.User.SendMessageAsync(
                    "Could not delete all the emotes.");
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

        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
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
                                }
                                else
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
                            newRaid.MessageId =
                                (await ReplyAsync(newRaid.BuildAllowedRoles(), false, newRaid.BuildEmbed())).Id;
                            guild.ActiveRaids.Add(new AdderChannel(Context.Channel.Id, newRaid));
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
            var guildId = Context.Guild.Id;

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == guildId);
            if (guild == null)
            {
                await Context.User.SendMessageAsync(
                    "The guild did not get added somehow. Contact the developer for help.");
            }

            else
            {
                var adderChannel = guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id);
                if (adderChannel == null)
                {
                    await Context.User.SendMessageAsync(
                        $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name}");
                }
                else
                {
                    var adderPlayer =
                        adderChannel.Raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == Context.User.Id);
                    if (adderPlayer != null)
                    {
                        adderChannel.Raid.CurrentPlayers.Remove(adderPlayer);
                        await ((IUserMessage) await Context.Channel.GetMessageAsync(adderChannel.Raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = adderChannel.Raid.BuildEmbed();
                                x.Content = adderChannel.Raid.BuildAllowedRoles();
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

        [Command("raid-remove")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidRemoveAsync(string user)
        {
            var parsedUser = ulong.Parse(user.ToLower().Trim().Substring(2, user.Length - 3));

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

        [Command("raid-add")]
        [Summary("Adds user as role (override)")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidAddAsync(string user, string role, [Remainder] string emote = null)
        {
            var parsedUser = ulong.Parse(user.Trim().Substring(2, user.Length - 3));

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
                                    adderChannel.Raid.AddPlayer(parsedUser, parsedRole, CheckEmoteValid(emote, Context).Result);
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

        [Command("help")]
        [Summary("Sends the help message")]
        public async Task HelpAsync()
        {
            var isAdmin = false;
            var isTrialLead = false;

            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == Context.Guild.Id);
            if (guild == null || ((SocketGuildUser) Context.User).GuildPermissions.Administrator)
            {
                isAdmin = true;
                isTrialLead = true;
            }
            else
            {
                var adderChannel = guild.ActiveRaids.FirstOrDefault(x => x.Raid.Lead == Context.User.Id);
                if (adderChannel != null || ((SocketGuildUser) Context.User).Roles.Count(x => x.Id == guild.Lead) != 0)
                {
                    isTrialLead = true;
                }
            }

            var embedBuilder = new EmbedBuilder();
            if (isAdmin)
            {
                embedBuilder.Color = Color.Green;
                embedBuilder.Description = CommandHelp.AdminCommandHelp.Representation;
                await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
            }

            if (isTrialLead)
            {
                embedBuilder.Color = Color.Purple;
                embedBuilder.Description = CommandHelp.RaidLeadCommandHelp.Representation;
                await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
            }

            embedBuilder.Color = Color.Blue;
            embedBuilder.Description = CommandHelp.BasicCommandHelp.Representation;
            await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
        }

        public static async Task UpdateRoster(SocketCommandContext scc, Role role, string emote)
        {
            var guild = Adderbot.Data.Guilds.FirstOrDefault(x => x.GuildId == scc.Guild.Id);
            if (guild == null)
            {
                await scc.User.SendMessageAsync("Somehow the guild did not get added. Contact the developer for help.");
            }
            else
            {
                var adderChannel = guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == scc.Channel.Id);
                if (adderChannel == null)
                {
                    await scc.User.SendMessageAsync("There is not a raid in this channel!");
                }
                else
                {
                    if (!adderChannel.Raid.AllowedRoles.Contains(0)
                        && ((SocketGuildUser) scc.User).Roles.FirstOrDefault(x =>
                            adderChannel.Raid.AllowedRoles.Contains(x.Id)) == null)
                    {
                        await scc.User.SendMessageAsync(
                            $"You aren't allowed to join this raid, please message <@{adderChannel.Raid.Lead}> for more information.");
                    }
                    else if (adderChannel.Raid.CurrentPlayers.Count(x => x.PlayerId == scc.User.Id) != 0)
                    {
                        await scc.User.SendMessageAsync(
                            $"You already joined this raid, if you want to change your role, remove" +
                            $" yourself from it and add yourself again.");
                    }
                    else
                    {
                        try
                        {
                            adderChannel.Raid.AddPlayer(scc.User.Id, role, CheckEmoteValid(emote, scc).Result);
                            await ((IUserMessage) await scc.Channel.GetMessageAsync(adderChannel.Raid.MessageId))
                                .ModifyAsync(x =>
                                {
                                    x.Embed = adderChannel.Raid.BuildEmbed();
                                    x.Content = adderChannel.Raid.BuildAllowedRoles();
                                });
                        }
                        catch (ArgumentException ae)
                        {
                            await scc.User.SendMessageAsync(ae.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
        }

        [Command("dps")]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DpsAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Context, Role.Dps, emote);
        }
    }
}