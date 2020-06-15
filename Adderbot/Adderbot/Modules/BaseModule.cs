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
    internal class BaseModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Retrieves the guild associated with the command. 
        /// </summary>
        /// <returns></returns>
        protected async Task<AdderGuild> GetGuild()
        {
            var guild = GuildHelper.GetGuildById(Context.Guild.Id);
            if (guild != null) return guild;
            await Context.User.SendMessageAsync(MessageText.Error.InvalidGuild);
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
            await Context.User.SendMessageAsync(MessageText.Error.InvalidRaid);
            return null;
        }
        
        /// <summary>
        /// Get role, if exists, from a string
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        protected async Task<SocketRole> GetRoleFromString(string role)
        {
            return Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(role.Trim()));
        }

        /// <summary>
        /// Validate the the user who sent message is the lead of the raid in the channel
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        protected async Task<bool> ValidateUseHasRaidLeadRole(AdderGuild guild)
        {
            if (guild.Lead == 0 || ((SocketGuildUser) Context.User).Roles.FirstOrDefault(
                                    x => x.Id == guild.Lead) != null
                                || ((SocketGuildUser) Context.User).GuildPermissions.Administrator) return true;
            await Context.User.SendMessageAsync(MessageText.Error.HasNoRaidLeadRole);
            return false;
        }
        
        /// <summary>
        /// Validates that the channel the message was sent in has no raid 
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        protected async Task<bool> ValidateChannelIsEmpty(AdderGuild guild)
        {
            if (guild.ActiveRaids.FirstOrDefault(x => x.ChannelId == Context.Channel.Id) == null) return true;
            await Context.User.SendMessageAsync(
                $"There is already a raid in {Context.Guild.Name}/{Context.Channel.Name}");
            return false;
        }
        
        /// <summary>
        /// Validates that the user is the lead of the raid in the channel message is sent in
        /// </summary>
        /// <param name="adderRaid"></param>
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
        /// <param name="emote"></param>
        /// <param name="scc"></param>
        /// <returns></returns>
        protected static async Task<Emote> ValidateEmoteValid(string emote, SocketCommandContext scc)
        {
            if (emote == null) return null;
            if (!Emote.TryParse(emote, out var parsedEmote))
            {
                await scc.User.SendMessageAsync(
                    "Emote not available. You'll still be added to the raid, but without the emote");
                return null;
            }

            if (scc.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name))
                .Any(guildEmote => guildEmote.Id == parsedEmote.Id))
            {
                return parsedEmote;
            }

            await scc.User.SendMessageAsync(
                "Emote not available. You'll still be added to the raid, but without the emote");

            return parsedEmote;
        }

        protected async Task<IMessage> GetMessageById(ulong messageId)
        {
            var raidMessage = await Context.Channel.GetMessageAsync(messageId);
            if (raidMessage != null) return raidMessage;
            await Context.User.SendMessageAsync(MessageText.Error.InvalidRaid);
            return null;
        }

        public static async Task TimeDeleteMessage(IUserMessage message, int timeInMs)
        {
            await Task.Delay(timeInMs);
            await message.DeleteAsync();
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
                            adderChannel.Raid.AddPlayer(scc.User.Id, role, ValidateEmoteValid(emote, scc).Result);
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

        protected static ulong ParseUser(string userMention)
        {
            var startIdx = userMention.IndexOfAny("0123456789".ToCharArray());
            var endIdx = userMention.IndexOf('>');
            var intStr = userMention.Substring(startIdx, userMention.Length - startIdx - 1);
            return ulong.Parse(intStr);
        }

        [Command("dps")]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DpsAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Context, Role.Dps, emote);
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

        [Command("flip")]
        [Summary("Flips the coin")]
        public async Task FlipCoinAsync()
        {
            await TimeDeleteMessage(Adderbot.randomGen.Next(0, 2) == 0 ? (await ReplyAsync("Heads!")) : (await ReplyAsync("Tails!")), 10000);
        }
    }
}