using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    /// <summary>
    /// Module of all administrative commands
    /// </summary>
    internal class AdminModule : BaseModule
    {
        /// <summary>
        /// Handles ;purge
        /// Purges up to 100 messages in the channel
        /// </summary>
        /// <returns></returns>
        [Command("purge")]
        [Summary("Purges channel of all messages")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeAsync()
        {
            // Retrieve the AdderGuild
            var guild = GetGuild().Result;

            if (guild != null)
            {
                // Get the AdderChannel object in the purged channel (if it exists)
                var ac = guild.ActiveRaids.FirstOrDefault(x => (x.ChannelId == Context.Channel.Id));

                // If the AdderChannel object is null, remove it from the ActiveRaids list for the guild
                if (ac != null) guild.ActiveRaids.Remove(ac);

                // Retrieve 100 messages and delete them
                var messages = await Context.Channel.GetMessagesAsync().FlattenAsync();
                await ((ITextChannel) Context.Channel).DeleteMessagesAsync(messages);
                
                // Post purge message
                var purgeMessage = await ReplyAsync("Purge completed. _This message will be deleted in 5 seconds_");
                
                // Iterate once a second and update purge message
                for (var i = 4; i > 0; i--)
                {
                    await Task.Delay(1000);
                    await purgeMessage.ModifyAsync(x =>
                        x.Content = $"Purge completed. _This message will be deleted in {i} seconds_");
                }

                // Fully delete the purge message
                await purgeMessage.DeleteAsync();
            }
        }

        /// <summary>
        /// Handles ;set-debug
        /// Turns debug mode on and off. Resets on bot reboot. Only usable by the Developer.
        /// </summary>
        /// <returns></returns>
        [Command("set-debug")]
        [Summary("Flips the debug switch, can turn on and off")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetDebug()
        {
            // Check if user is the developer
            if (Context.User.Id == Adderbot.DevId)
            {
                // Flip switch and send message
                Adderbot.InDebug = !Adderbot.InDebug;
                await SendMessageToUser($"Set bot debug to {Adderbot.InDebug}");
                await Task.CompletedTask;
            }
            else
            {
                await SendMessageToUser(MessageText.Error.CommandUnavailable);
            }
        }

        /// <summary>
        /// Handles ;setleadrole
        /// Sets the role used to create raids
        /// </summary>
        /// <param name="userRole"></param>
        /// <returns></returns>
        [Command("setleadrole")]
        [Summary("Sets the role to be used as trial lead")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task SetLeadRole([Remainder] string userRole)
        {
            // Get the role from its string representation 
            var role = GetRoleFromString(userRole);
            
            if (role != null)
            {
                // Get the guild
                var guild = GetGuild().Result;
                // Retrieve the lead role id
                guild.Lead = role.Id;
                
                await SendMessageToUser($"Successfully set trial lead role to {userRole} in {Context.Guild.Name}");
            }
            else
            {
                await SendMessageToUser($"Unable to set trial lead role to {userRole} in {Context.Guild.Name}");
            }
        }
    }
}