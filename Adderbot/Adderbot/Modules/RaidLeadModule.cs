using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    internal class RaidLeadModule : BaseModule
    {
        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
        {
            var raid = GetRaid().Result;
            if (raid != null && CheckUserIsLead(raid).Result)
            {
                var socketRole =
                    Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals(userRole.ToLower()));
                if (socketRole == null)
                {
                    await Context.User.SendMessageAsync("Please enter a valid role!");
                }
                else
                {
                    var raidMessage = GetMessageById(raid.MessageId).Result;
                    if(raidMessage != null)
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