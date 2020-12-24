using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    public class EmoteModule : BaseModule
    {
        /// <summary>
        /// Handles ;check-emotes
        /// Checks what emotes are available in the server (if any)
        /// </summary>
        /// <returns></returns>
        [Command("check-emotes")]
        [Summary("Checks which emotes are available")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task CheckEmotes()
        {
            if (GetGuild().Result != null && GuildHelper.CheckEmoteAvailability(Context.Guild))
            {
                await ReplyAsync(
                    $"Available Emotes\n{Context.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name)).Aggregate("", (current, emote) => current + $"{emote} - {emote.Name}\n")}");
            }
            else
            {
                await ReplyAsync(MessageText.Error.EmotesUnavailable);
            }
        }
        
        /// <summary>
        /// Handles ;add-emotes
        /// Adds all the emotes from the bot to the Discord guild
        /// </summary>
        /// <returns></returns>
        [Command("add-emotes")]
        [Summary("Attempts to add all the emotes the bot uses & enables them")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageEmojis)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddEmotes()
        {
            try
            {
                // Get the guild
                var guild = GetGuild().Result;
                if (guild != null)
                {
                    // Get the path of this executable
                    var path = Directory.GetCurrentDirectory();

                    // Spin through all emotes Adderbot supports
                    foreach (var emote in Adderbot.EmoteNames)
                    {
                        // If there is not an emote with the given name create it
                        if (Context.Guild.Emotes.Any(x => x.Name == emote)) continue;
                        
                        await Context.Guild.CreateEmoteAsync(emote,
                            new Image($@"{path}\Media\{emote}.png"));
                        await Task.Delay(1000);
                    }

                    // Set emote availability flag to true
                    guild.EmotesAvailable = true;
                    
                    await SendMessageToUser(MessageText.Success.EmoteAddSuccessful);
                }
            }
            catch (Exception)
            {
                await Context.User.SendMessageAsync(MessageText.Error.EmotesCannotBeAdded);
            }
        }
        
        /// <summary>
        /// Handles ;remove-emotes
        /// Attempts to remove all emotes the bot uses in the Discord guild
        /// </summary>
        /// <returns></returns>
        [Command("remove-emotes")]
        [Summary("Attempts to remove all the emotes the bot uses")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageEmojis)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveEmotes()
        {
            try
            {
                var guild = GetGuild().Result;
                if (guild != null)
                {
                    await SendMessageToUser(MessageText.Misc.LongCommandWarning);
                    
                    // Spin through all emotes Adderbot supports
                    foreach (var emote in Adderbot.EmoteNames)
                    {
                        // Get the emote if exists
                        var guildEmote = Context.Guild.Emotes.FirstOrDefault(x => x.Name != emote);
                        
                        // If there is not an emote with the given name create it
                        if (guildEmote == null) continue;
                        
                        await Context.Guild.DeleteEmoteAsync(guildEmote);
                        await Task.Delay(60000);
                    }

                    // Set emote availability flag to false
                    guild.EmotesAvailable = false;
                    
                    await SendMessageToUser(MessageText.Success.EmoteDeleteSuccessful);
                }
            }
            catch (Exception)
            {
                await Context.User.SendMessageAsync(MessageText.Error.EmotesCannotBeDeleted);
            }
        }
    }
}