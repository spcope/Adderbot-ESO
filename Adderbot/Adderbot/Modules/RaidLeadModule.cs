using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Adderbot.Modules
{
    public class RaidLeadModule : BaseModule
    {
        /// <summary>
        /// Handles ;summon
        /// Pings the roster with grouping information
        /// </summary>
        /// <returns></returns>
        [Command("summon")]
        [Summary("@s the raiders listed in the raid in the channel")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SummonRaiders()
        {
            // Retrieve the raid in the current channel
            var raid = GetRaid().Result;
            if (raid != null)
            {
                // Validate the user can edit the raid
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                    await ReplyAsync(
                        $"{raid.BuildPlayers()}{RaidTypeRepresentation.GetRepresentation(raid.Type)} led by <@{raid.Lead}> is forming. X up in guild or whisper them in game.");
            }
        }

        /// <summary>
        /// Handles ;create
        /// Creates a raid with the given parameters
        /// </summary>
        /// <param name="raidClass">The class of the raid (norm, vet, hard mode)</param>
        /// <param name="raidType">The type of the raid (AA, HRC, etc)</param>
        /// <param name="date">The date on which the raid will occur</param>
        /// <param name="time">The time when the raid will occur</param>
        /// <param name="timezone">The timezone when the raid will occur</param>
        /// <param name="mNum">The number of melee DPS</param>
        /// <param name="rNum">The number of ranged DPS</param>
        /// <param name="fNum">The number of flex DPS</param>
        /// <param name="nNum">The number of necro DPS</param>
        /// <param name="allowedRolesArgs">The allowed roles in a raid, separated by |</param>
        /// <returns></returns>
        [Command("create")]
        [Summary("Creates a raid.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CreateAsync(string raidClass, string raidType, string date, string time, string timezone,
            int mNum = 0, int rNum = 0, int fNum = -1, int nNum = 0, [Remainder] string allowedRolesArgs = null)
        {
            // Get the guild
            var guild = GetGuild().Result;

            if (guild != null)
            {
                // Check if channel is empty
                if (ValidateChannelIsEmpty(guild).Result)
                {
                    // Check if user has raid lead role
                    if (ValidateUserHasRaidLeadRole(guild).Result)
                    {
                        // List of roles allowed in the raid
                        var allowedRoles = new SynchronizedCollection<ulong>();

                        // If there are no allowed roles specified, add @everyone (ID: 0)
                        if (allowedRolesArgs == null)
                        {
                            allowedRoles.Add(0);
                        }
                        else
                        {
                            // Split by | character
                            var allowedRolesStrings = allowedRolesArgs.Split('|');
                            foreach (var allowedRoleString in allowedRolesStrings)
                            {
                                // Get the associated role from text representation
                                var foundRole = GetRoleFromString(allowedRoleString);

                                // Throw error if role is null
                                if (foundRole == null)
                                {
                                    await SendMessageToUser($"Could not add {allowedRoleString} as an allowed role.");
                                }
                                else
                                {
                                    // Check if role is in list, add if it is not
                                    if (!allowedRoles.Contains(foundRole.Id))
                                    {
                                        allowedRoles.Add(foundRole.Id);
                                    }
                                }
                            }
                        }

                        try
                        {
                            // Attempt to construct a raid from given requirements
                            var newRaid = new AdderRaid(raidClass, raidType.ToLower(), date, time, timezone,
                                Context.User.Id, mNum,
                                rNum, fNum, nNum, allowedRoles);
                            // Get the ID of the message created
                            newRaid.MessageId =
                                (await ReplyAsync(newRaid.BuildAllowedRoles(), false, newRaid.BuildEmbed())).Id;
                            // Add the raid to the list and save to the JSON file
                            guild.ActiveRaids.Add(new AdderChannel(Context.Channel.Id, newRaid));
                        }
                        catch (ArgumentException ae)
                        {
                            await SendMessageToUser(ae.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles ;delete
        /// Deletes the raid in the channel 
        /// </summary>
        /// <returns></returns>
        [Command("delete")]
        [Summary("Deletes raid associated with user.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteAsync()
        {
            // Get the AdderGuild
            var guild = GetGuild().Result;

            if (guild != null)
            {
                // Get the AdderRaid in the channel
                var activeRaid = GetRaid().Result;

                if (activeRaid == null)
                {
                    // If the raid is null, send error message
                    await Context.User.SendMessageAsync(MessageText.Error.RaidNotFoundForLead);
                }
                else
                {
                    if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, activeRaid))
                    {
                        // Try to get the message object for the raid
                        var raidMessage = GetMessageById(activeRaid.MessageId).Result;
                        if (raidMessage == null)
                        {
                            // If it can't find the message send error message
                            await Context.User.SendMessageAsync(MessageText.Error.InvalidRaid);
                        }
                        else
                        {
                            // Remove raid from backend
                            RaidHelper.RemoveRaid(guild, Context.Channel.Id, Context.User.Id);
                            // Delete the actual Discord message
                            await raidMessage.DeleteAsync();
                            // Send delete message
                            await SendMessageToUser(MessageText.Misc.DeleteSuccessful);
                        }
                    }
                    else
                    {
                        // Send raid not found message
                        await SendMessageToUser(MessageText.Error.RaidNotFoundForLead);
                    }
                }
            }
        }

        /// <summary>
        /// Handles ;addallowedrole
        /// Adds an allowed role to the raid
        /// </summary>
        /// <param name="userRole">String representation of a role</param>
        /// <returns></returns>
        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
        {
            // Get the raid in the channel
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    // Get the SocketRole representation of the string role
                    var socketRole = GetRoleFromString(userRole);

                    if (socketRole == null)
                    {
                        await SendMessageToUser(MessageText.Error.InvalidRole);
                    }
                    else
                    {
                        // Get the Discord message containing the raid
                        var raidMessage = GetMessageById(raid.MessageId).Result;

                        if (raidMessage != null)
                        {
                            if (!raid.AllowedRoles.Contains(socketRole.Id))
                            {
                                // If AllowedRoles does not contain the passed in role, add it to list
                                raid.AllowedRoles.Add(socketRole.Id);
                                // Remove Everyone role
                                if (raid.AllowedRoles.Contains(0)) raid.AllowedRoles.Remove(0);
                                // Send success message
                                await SendMessageToUser($"Added {userRole} to the list of allowed roles for the raid.");
                                // Redraw raid
                                await RedrawRaid();
                            }
                            else
                            {
                                // Send duplicate role message
                                await SendMessageToUser($"{userRole} was already in the list and couldn't be added.");
                            }
                        }
                    }
                }
                else
                {
                    // Send not a raid lead error message
                    await SendMessageToUser(MessageText.Error.HasNoRaidLeadRole);
                }
            }
        }

        /// <summary>
        /// Removes a role from the list of allowed roles
        /// </summary>
        /// <param name="userRole">String representation of the role</param>
        /// <returns></returns>
        [Command("removeallowedrole")]
        [Summary("Removes a user role from the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RemoveAllowedRoleAsync([Remainder] string userRole)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    // Get the role
                    var socketRole = GetRoleFromString(userRole);

                    if (socketRole == null)
                    {
                        await SendMessageToUser(MessageText.Error.InvalidRole);
                    }
                    else
                    {
                        // Get the raid message
                        var raidMessage = GetMessageById(raid.MessageId).Result;
                        if (raidMessage == null)
                        {
                            await Context.User.SendMessageAsync(
                                $"There is not a raid in {Context.Guild.Name}/{Context.Channel.Name} to set allowed roles for!");
                        }
                        else
                        {
                            // Remove the role from the list of allowed roles
                            raid.AllowedRoles.Remove(socketRole.Id);
                            // If that empties the list, add the Everyone role
                            if (!raid.AllowedRoles.Any()) raid.AllowedRoles.Add(0);

                            // Send remove role message
                            await Context.User.SendMessageAsync(
                                $"Removed {userRole} from the list of allowed roles for the raid.");
                            // Redraw
                            await RedrawRaid();
                        }
                    }
                }
                else
                {
                    // Send error message
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }

        /// <summary>
        /// Handles ;raid-add
        /// Forcefully adds a user to the raid with the given emote
        /// </summary>
        /// <param name="userId">String of user @ mention</param>
        /// <param name="role">String of the role name</param>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("raid-add")]
        [Summary("Adds user as role (override)")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidAddAsync(string userId, string role, [Remainder] string emote = null)
        {
            // Get the user object from the guild
            var user = Context.Guild.GetUser(MentionUtils.ParseUser(userId));

            // Get the raid
            var raid = GetRaid().Result;
            if (raid != null && user != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    // Check current players for user
                    if (raid.CurrentPlayers.All(x => x.PlayerId != user.Id))
                    {
                        // Get the Role from the string representation of the role
                        var parsedRole = RoleHelper.GetRoleFromString(role);
                        try
                        {
                            // Add the player to the raid
                            raid.AddPlayer(user.Id, parsedRole,
                                ValidateEmoteValid(emote).Result);
                        }
                        catch (ArgumentException ae)
                        {
                            // If AddPlayer throws, send error to user
                            await SendMessageToUser(ae.Message);
                            return;
                        }

                        // Send message to the user being added telling them they've been added
                        await user.SendMessageAsync($"You have been added to the raid in {Context.Channel.Name}.");
                        // Send the message to the raid lead telling them the user was added
                        await SendMessageToUser(
                            $"Added {MentionUtils.MentionUser(user.Id)} to the raid in {Context.Channel.Name}");

                        // Redraw raid
                        await RedrawRaid();
                    }
                    else
                    {
                        // Send error message to raid lead
                        await SendMessageToUser(
                            $"{user} was on the raid roster already and could not be added.");
                    }
                }
                else
                {
                    // Send error message to raid lead
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }

        /// <summary>
        /// Handles ;raid-remove
        /// Forcefully removes a user from the raid with the given emote
        /// </summary>
        /// <param name="user">String of user @ mention</param>
        /// <returns></returns>
        [Command("raid-remove")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidRemoveAsync(string user)
        {
            // Parse the user @ mention
            var parsedUser = MentionUtils.ParseUser(user);

            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    // Get the players from the raid
                    var player = raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == parsedUser);
                    if (player == null)
                    {
                        await SendMessageToUser($"The raid in the channel does not have {MentionUtils.MentionUser(parsedUser)} in it");
                    }
                    else
                    {
                        // Remove player from raid
                        raid.CurrentPlayers.Remove(player);
                        if (!Adderbot.InDebug)
                            await Context.Guild.GetUser(parsedUser)
                                .SendMessageAsync(
                                    $"You have been removed from the raid in {Context.Channel.Name}.");

                        await Context.User.SendMessageAsync(
                            $"Successfully removed {MentionUtils.MentionUser(parsedUser)} from the raid in {Context.Channel.Name}");

                        // Redraw raid
                        await RedrawRaid();
                    }
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }

        /// <summary>
        /// Handles ;alter-time
        /// Alters the time of the raid
        /// </summary>
        /// <param name="time">String representation of time</param>
        /// <returns></returns>
        [Command("alter-time")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AlterTime(string time)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    var dt = TimeHelper.ParseTime(raid.Date, time, raid.Timezone);
                    if (dt == DateTime.UnixEpoch)
                    {
                        await SendMessageToUser(MessageText.Error.InvalidTime);
                    }
                    else
                    {
                        raid.Time = time;
                        raid.DateTimeObj = dt;
                        // Redraw raid
                        await RedrawRaid();
                    }
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }
        
        /// <summary>
        /// Handles ;alter-timezone
        /// Alters the timezone of a raid
        /// </summary>
        /// <param name="timezone">String representation of the timezone</param>
        /// <returns></returns>
        [Command("alter-timezone")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AlterTimezone(string timezone)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    var dt = TimeHelper.ParseTime(raid.Date, raid.Time, timezone);
                    if (dt == DateTime.UnixEpoch)
                    {
                        await SendMessageToUser(MessageText.Error.InvalidTimezone);
                    }
                    else
                    {
                        raid.Timezone = timezone;
                        raid.DateTimeObj = dt;
                        // Redraw raid
                        await RedrawRaid();
                    }
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }
        
        /// <summary>
        /// Handles ;alter-date
        /// Alters the date of a raid
        /// </summary>
        /// <param name="date">String representation of the date</param>
        /// <returns></returns>
        [Command("alter-date")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AlterDate(string date)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    var dt = TimeHelper.ParseTime(date, raid.Time, raid.Timezone);
                    if (dt == DateTime.UnixEpoch)
                    {
                        await SendMessageToUser(MessageText.Error.InvalidTimezone);
                    }
                    else
                    {
                        raid.Date = date;
                        raid.DateTimeObj = dt;
                        // Redraw raid
                        await RedrawRaid();
                    }
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }
        
        /// <summary>
        /// Handles ;alter-headline
        /// Alters the date of a headline. Sets the override to true
        /// </summary>
        /// <param name="headline">String representation of the headline</param>
        /// <returns></returns>
        [Command("alter-headline")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AlterHeadline(string headline)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    raid.HeadlineOverride = true;
                    raid.OverridenHeadline = headline;
                    // Redraw raid
                    await RedrawRaid();
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }
        
        /// <summary>
        /// Handles ;reset-headline
        /// Alters the date of a headline. Sets the override to true
        /// </summary>
        /// <returns></returns>
        [Command("reset-headline")]
        [Summary("Removes someone from a raid roster")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ResetHeadline()
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (await GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    raid.HeadlineOverride = false;
                    raid.OverridenHeadline = string.Empty;
                    // Redraw raid
                    await RedrawRaid();
                }
                else
                {
                    // Send the error message to the user
                    await SendMessageToUser(MessageText.Error.NotRaidLead);
                }
            }
        }
    }
}