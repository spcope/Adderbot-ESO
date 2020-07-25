using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        protected SocketRole GetRoleFromString(string role)
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
        protected async Task SendMessageToUser(string message)
        {
            await Context.User.SendMessageAsync(message);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Validate the the user who sent message is a raid lead
        /// </summary>
        /// <param name="guild">The guild to validate against</param>
        /// <returns></returns>
        protected async Task<bool> ValidateUserHasRaidLeadRole(AdderGuild guild)
        {
            // Look if the user has the lead role
            if (guild.Lead == 0 || ((SocketGuildUser) Context.User).Roles.FirstOrDefault(
                                    x => x.Id == guild.Lead) != null
                                || GuildHelper.IsUserAdminInGuild(Context.Guild, Context.User)) return true;
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
        /// Validates that the user is the lead of the raid in the channel message is sent in
        /// </summary>
        /// <param name="adderRaid">The raid to validate against</param>
        /// <returns></returns>
        protected async Task<bool> ValidateUserIsLead(AdderRaid adderRaid)
        {
            if (RaidHelper.CheckUserIsLead(Context.User.Id, adderRaid)) return true;
            await Context.User.SendMessageAsync(MessageText.Error.NotRaidLead);
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
            if (Context.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name))
                .Any(guildEmote => guildEmote.Id == parsedEmote.Id))
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
                if (!(raid.AllowedRoles.Contains(0)
                      || ((SocketGuildUser) Context.User).Roles.Any(x =>
                          raid.AllowedRoles.Contains(x.Id))))
                {
                    // If the raid is not @everyone or does not have an AllowedRole
                    await SendMessageToUser(
                        $"You aren't allowed to join this raid, please message <@{raid.Lead}> for more information.");
                }
                else if (raid.CurrentPlayers.Count(x => x.PlayerId == Context.User.Id) != 0)
                {
                    // Send error message if user is already in the raid
                    await SendMessageToUser(MessageText.Error.AlreadyInRaid);
                }
                else
                {
                    try
                    {
                        // Add the new player to the raid
                        raid.AddPlayer(Context.User.Id, role, ValidateEmoteValid(emote).Result);
                        // Redraw raid
                        await ((IUserMessage) await Context.Channel.GetMessageAsync(raid.MessageId))
                            .ModifyAsync(x =>
                            {
                                x.Embed = raid.BuildEmbed();
                                x.Content = raid.BuildAllowedRoles();
                            });
                    }
                    catch (ArgumentException ae)
                    {
                        // If AddPlayer ran into error, send error message
                        await SendMessageToUser(ae.Message);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Parses the user mention into a user's id
        /// </summary>
        /// <param name="userMention">String of the user mention</param>
        /// <returns>0 if unparseable, the ulong id of the Discord user if parseable</returns>
        protected static ulong ParseUser(string userMention)
        {
            // Fish out the numbers out of the user mention
            var intStr = Regex.Match(userMention, @"\d+").Value;
            return string.IsNullOrEmpty(intStr) ? 0 : ulong.Parse(intStr);
        }
    }
}