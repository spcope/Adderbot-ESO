using System;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// NOTE: This file is the one module that does not contain any commands

namespace Adderbot.Modules
{
    public class BaseModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Retrieves the guild associated with the command. 
        /// </summary>
        /// <returns></returns>
        protected async Task<AdderGuild> GetGuild()
        {
            var guild = GuildHelper.GetGuildById(Context.Guild.Id);
            if (guild != null) return guild;
            await SendMessageToUser(MessageText.Error.InvalidGuild);
            return null;
        }

        /// <summary>
        /// Gets the raid in the channel the message is sent in, if it exists.
        /// </summary>
        /// <returns></returns>
        protected async Task<AdderRaid> GetRaid()
        {
            var guild = GetGuild().Result;
            if (guild == null) return null;
            var raid = RaidHelper.GetRaidByChannelId(guild, Context.Channel.Id);
            if (raid != null) return raid;
            await SendMessageToUser(MessageText.Error.InvalidRaid);
            return null;
        }

        /// <summary>
        /// Get role, if exists, from a string
        /// </summary>
        /// <param name="role">The string representation of the role</param>
        /// <returns></returns>
        protected IRole GetRoleFromString(string role)
        {
            return Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(role.Trim()));
        }

        /// <summary>
        /// Get the Discord message associated with messageId
        /// </summary>
        /// <param name="messageId">The id of the Discord message</param>
        /// <returns></returns>
        protected async Task<IMessage> GetMessageById(ulong messageId)
        {
            // Try to get the message
            var raidMessage = await Context.Channel.GetMessageAsync(messageId);
            if (raidMessage != null) return raidMessage;
            // Return invalid raid message
            await Context.User.SendMessageAsync(MessageText.Error.InvalidRaid);
            return null;
        }

        /// <summary>
        /// Sends a message to the user
        /// </summary>
        /// <param name="message">The message to send to the user</param>
        protected async Task<IUserMessage> SendMessageToUser(string message)
        {
            return await Context.User.SendMessageAsync(message);
        }

        /// <summary>
        /// Validate the the user who sent message is a raid lead
        /// </summary>
        /// <param name="guild">The guild to validate against</param>
        /// <returns></returns>
        protected async Task<bool> ValidateUserHasRaidLeadRole(AdderGuild guild)
        {
            if (Context.User is SocketGuildUser user)
            {
                // Look if the user has the lead role
                if (guild.Lead == 0 || user.Roles.FirstOrDefault(
                                        x => x.Id == guild.Lead) != null
                                    || await GuildHelper.IsUserAdminInGuild(Context.Guild, Context.User)) return true;
            }

            // Send error message
            await SendMessageToUser(MessageText.Error.HasNoRaidLeadRole);
            return false;
        }

        /// <summary>
        /// Validates that the channel the message was sent in has no raid 
        /// </summary>
        /// <param name="guild">The guild to validate against</param>
        /// <returns></returns>
        protected async Task<bool> ValidateChannelIsEmpty(AdderGuild guild)
        {
            // Check if channel is empty
            if (guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id) == null) return true;
            // Send error message
            await SendMessageToUser($"There is already a raid in {Context.Guild.Name}/{Context.Channel.Name}");
            return false;
        }

        /// <summary>
        /// Validates if the emote string is valid
        /// </summary>
        /// <param name="emote">The string representation of the emote</param>
        /// <returns></returns>
        protected async Task<Emote> ValidateEmoteValid(string emote)
        {
            // Null check
            if (emote == null) return null;

            // Attempt to parse emote
            if (!Emote.TryParse(emote, out var parsedEmote))
            {
                await SendMessageToUser(MessageText.Error.EmoteUnparseable);
                return null;
            }

            // Get the emote in the Discord guild
            if ((await Context.Guild.GetEmoteAsync(parsedEmote.Id)) != null)
            {
                return parsedEmote;
            }

            // Send error message
            await SendMessageToUser(MessageText.Error.EmoteInvalid);
            // Return null
            return null;
        }

        /// <summary>
        /// Deletes a message after timeInMs milliseconds
        /// </summary>
        /// <param name="message">The Discord message to be deleted</param>
        /// <param name="timeInMs">The time in ms until message is deleted</param>
        /// <returns></returns>
        public async Task TimeDeleteMessage(IUserMessage message, int timeInMs)
        {
            if (message == null || timeInMs < 0) return;
            await Task.Delay(timeInMs);
            await message.DeleteAsync();
        }

        /// <summary>
        /// Updates the roster with the current user, role, and emote
        /// </summary>
        /// <param name="role">The role of the user to add</param>
        /// <param name="emote">The string representation of the emote</param>
        /// <returns></returns>
        public async Task UpdateRoster(Role role, string emote)
        {
            // Get the raid
            var raid = GetRaid().Result;

            if (raid != null)
            {
                if (Context.User is SocketGuildUser user)
                {
                    if (!(raid.AllowedRoles.Contains<ulong>(0)
                          || user.Roles.Any(x =>
                              raid.AllowedRoles.Contains(x.Id))))
                    {
                        // If the raid is not @everyone or does not have an AllowedRole
                        await SendMessageToUser(
                            $"You aren't allowed to join this raid, please message <@{raid.Lead}> for more information.");
                    }
                    else if (raid.CurrentPlayers.Any(x => x.PlayerId == Context.User.Id))
                    {
                        // Send error message if user is already in the raid
                        await SendMessageToUser(MessageText.Error.AlreadyInRaid);
                    }
                    else
                    {
                        try
                        {
                            // Add the new player to the raid
                            raid.AddPlayer(user.Id, role, ValidateEmoteValid(emote).Result);
                            // Redraw raid
                            await RedrawRaid();
                        }
                        catch (ArgumentException ae)
                        {
                            // If AddPlayer ran into error, send error message
                            await SendMessageToUser(ae.Message);
                        }
                        catch (Exception e)
                        {
                            await Console.Error.WriteLineAsync(e.Message);
                        }
                    }
                }
            }
            else
            {
                await SendMessageToUser(MessageText.Error.InvalidRaid);
            }
        }

        /// <summary>
        /// Handles ;summon
        /// Pings the roster with grouping information
        /// </summary>
        /// <returns></returns>
        [Command("redraw")]
        [Summary("Redraws the raid")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task RedrawRaid()
        {
            // Retrieve the raid in the current channel
            var raid = GetRaid().Result;
            if (raid != null)
            {
                await ((IUserMessage) (await Context.Channel.GetMessageAsync(
                        raid.MessageId)))
                    .ModifyAsync(x =>
                    {
                        x.Embed = raid.BuildEmbed();
                        x.Content = raid.BuildAllowedRoles();
                    });
            }
        }
    }
}