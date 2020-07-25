using System.Linq;
using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    public class GenericModule : BaseModule
    {
        /// <summary>
        /// Handles ;remove
        /// Removes the user from the raid
        /// </summary>
        /// <returns></returns>
        [Command("remove")]
        [Summary("Removes the user from the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RemoveAsync()
        {
            // Get the raid
            var raid = GetRaid().Result;
            // Retrieve the AdderPlayer for the current raid, if they are in it
            var adderPlayer = raid.CurrentPlayers.FirstOrDefault(x => x.PlayerId == Context.User.Id);
            if (adderPlayer != null)
            {
                // Remove the player from the raid
                raid.CurrentPlayers.Remove(adderPlayer);
                // Redraw raid
                await ((IUserMessage) await Context.Channel.GetMessageAsync(raid.MessageId))
                    .ModifyAsync(x =>
                    {
                        x.Embed = raid.BuildEmbed();
                        x.Content = raid.BuildAllowedRoles();
                    });
            }
        }

        /// <summary>
        /// Handles ;help
        /// Sends the help message to the user based on the role
        /// </summary>
        /// <returns></returns>
        [Command("help")]
        [Summary("Sends the help message")]
        public async Task HelpAsync()
        {
            // Flag if user is an admin
            var isAdmin = false;
            // Flag if the user is a raid lead
            bool isTrialLead;

            // Get the guild 
            var guild = GetGuild().Result;
            
            if (guild != null)
            {
                if (GuildHelper.IsUserAdminInGuild(Context.Guild, Context.User))
                {
                    // If user is admin, set admin and trial lead flags
                    isAdmin = true;
                    isTrialLead = true;
                }
                else
                {
                    // If not admin check if user is a raid lead
                    isTrialLead = ValidateUserHasRaidLeadRole(guild).Result;
                }

                // Create EmbedBuilder
                var embedBuilder = new EmbedBuilder();
                if (isAdmin)
                {
                    // If admin flag is set, send help to user
                    embedBuilder.Color = Color.Green;
                    embedBuilder.Description = CommandHelp.AdminCommandHelp.Representation;
                    await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
                }

                if (isTrialLead)
                {
                    // If raid lead flag is set, send help to user
                    embedBuilder.Color = Color.Purple;
                    embedBuilder.Description = CommandHelp.RaidLeadCommandHelp.Representation;
                    await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
                }

                // Send general help to user
                embedBuilder.Color = Color.Blue;
                embedBuilder.Description = CommandHelp.BasicCommandHelp.Representation;
                await Context.User.SendMessageAsync(null, false, embedBuilder.Build());
            }
        }
    }
}