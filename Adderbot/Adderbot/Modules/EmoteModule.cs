using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    internal class EmoteModule : BaseModule
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
            var guild = GetGuild().Result;
            if (guild != null && guild.EmotesAvailable)
            {
                await ReplyAsync(
                    $"Available Emotes\n{Context.Guild.Emotes.Where(x => Adderbot.EmoteNames.Contains(x.Name)).Aggregate("", (current, emote) => current + $"{emote} - {emote.Name}\n")}");
            }
            else
            {
                await ReplyAsync(MessageText.Error.EmotesUnavailable);
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
                var guild = GetGuild().Result;
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
                var guild = GetGuild().Result;
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
    }
}