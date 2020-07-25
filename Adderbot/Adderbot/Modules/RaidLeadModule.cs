using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    internal class RaidLeadModule : BaseModule
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
                if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
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
                        var allowedRoles = new List<ulong>();

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
                    if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, activeRaid))
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
                if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
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
                                await ((IUserMessage) raidMessage)
                                    .ModifyAsync(x =>
                                    {
                                        x.Embed = raid.BuildEmbed();
                                        x.Content = raid.BuildAllowedRoles();
                                    });
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
                if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
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
                            await ((IUserMessage) raidMessage).ModifyAsync(x =>
                            {
                                x.Embed = raid.BuildEmbed();
                                x.Content = raid.BuildAllowedRoles();
                            });
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
        /// <param name="user">String of user @ mention</param>
        /// <param name="role">String of the role name</param>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("raid-add")]
        [Summary("Adds user as role (override)")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RaidAddAsync(string user, string role, [Remainder] string emote = null)
        {
            // Retrieve user id of the user mention
            var parsedUser = ParseUser(user);

            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    if (Context.Guild.Users.All(x => x.Id != parsedUser) && !Adderbot.InDebug)
                    {
                        await SendMessageToUser($"The user {user} doesn't exist in the channel.");
                    }
                    else
                    {
                        // Check current players for user
                        if (raid.CurrentPlayers.All(x => x.PlayerId != parsedUser))
                        {
                            // Get the Role from the string representation of the role
                            var parsedRole = RoleHelper.GetRoleFromString(role);
                            try
                            {
                                // Add the player to the raid
                                raid.AddPlayer(parsedUser, parsedRole,
                                    ValidateEmoteValid(emote).Result);

                                // Redraw raid
                                await ((IUserMessage) await Context.Channel.GetMessageAsync(raid
                                        .MessageId))
                                    .ModifyAsync(x =>
                                    {
                                        x.Embed = raid.BuildEmbed();
                                        x.Content = raid.BuildAllowedRoles();
                                    });
                            }
                            catch (ArgumentException ae)
                            {
                                // If AddPlayer throws, send error to user
                                await SendMessageToUser(ae.Message);
                                return;
                            }

                            // Send message to the user being added telling them they've been added
                            await Context.Guild.GetUser(parsedUser)
                                .SendMessageAsync($"You have been added to the raid in {Context.Channel.Name}.");
                            // Send the message to the raid lead telling them the user was added
                            await SendMessageToUser(
                                $"Added {user} to the raid in {Context.Channel.Name}");

                            // Redraw raid
                            await ((IUserMessage) (await Context.Channel.GetMessageAsync(
                                    raid.MessageId)))
                                .ModifyAsync(x =>
                                {
                                    x.Embed = raid.BuildEmbed();
                                    x.Content = raid.BuildAllowedRoles();
                                });
                        }
                        else
                        {
                            // Send error message to raid lead
                            await SendMessageToUser(
                                $"{user} was on the raid roster already and could not be added.");
                        }
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
            var parsedUser = ParseUser(user);

            // Get the raid
            var raid = GetRaid().Result;
            
            if (raid != null)
            {
                if (GuildHelper.IsUserRaidEditor(Context.Guild, Context.User, raid))
                {
                    // Get the players from the raid
                    var player = raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == parsedUser);
                    if (player == null)
                    {
                        await SendMessageToUser( $"The raid in the channel does not have <@{parsedUser}> in it");
                    }
                    else
                    {
                        // Remove player from raid
                        raid.CurrentPlayers.Remove(player);
                        if (!Adderbot.InDebug)
                            await Context.Guild.GetUser(parsedUser)
                                .SendMessageAsync(
                                    $"You have been removed from the raid in {Context.Channel.Name}.");
                        
                        // Redraw raid
                        await ((IUserMessage) await Context.Channel.GetMessageAsync(raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = raid.BuildEmbed();
                                x.Content = raid.BuildAllowedRoles();
                            });
                    }
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