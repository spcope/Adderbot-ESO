using Adderbot.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adderbot
{
    class BaseModule : ModuleBase<SocketCommandContext>
    {
        [Command("setleadrole")]
        [Summary("Sets the role to be used as trial lead")]
        public async Task SetLeadRole([Remainder] string userRole = null)
        {
            bool success = false;
            foreach (var role in Context.Guild.Roles)
            {
                if (userRole.ToLower().Equals(role.Name.ToLower()))
                {
                    Adderbot.trialLead = role;
                    success = true;
                    break;
                }
            }
            if (success)
                await Context.User.SendMessageAsync($"Successfully set trial lead role to {userRole}");
            else
                await Context.User.SendMessageAsync($"Unable to set trial lead role to {userRole}");
            await Context.Message.DeleteAsync();
        }

        [Command("addallowedrole")]
        [Summary("Adds a user role to the allowed list for the raid.")]
        public async Task AddAllowedRoleAsync([Remainder] string userRole)
        {
            Raid r = Adderbot.raids.GetValueOrDefault(Context.Channel.Id);
            if(r.lead != Context.User.Id)
            {
                await Context.User.SendMessageAsync("You are not the lead for this raid and cannot add allowed roles!");
            }
            else
            {
                SocketRole sr = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(userRole));
                if (sr == null)
                {
                    await Context.User.SendMessageAsync("Please enter a valid role!");
                }
                else
                {
                    r.restrictedRoles.Add(sr);
                    await Context.User.SendMessageAsync($"Added {userRole} to the list of allowed roles for the raid.");
                }
            }
            await Context.Message.DeleteAsync();
        }

        [Command("create")]
        [Summary("Creates a raid.")]
        public async Task CreateAsync(string rclass, string rtype, string date, string time, string timezone, bool noMelee, [Remainder] string progRole = null)
        {
            if (!((SocketGuildUser) Context.User).Roles.Contains(Adderbot.trialLead) 
                && !((SocketGuildUser) Context.User).GuildPermissions.Administrator)
            {
                await Context.User.SendMessageAsync("You do not have permission to create raids!");
                await Context.Message.DeleteAsync();
                return;
            }

            SocketRole sr;

            if (progRole == null)
            {
                sr = Context.Guild.EveryoneRole;
            }
            else
            {
                sr = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals(progRole)) ?? Context.Guild.EveryoneRole;
            }
            Raid r = new Raid(Context.User.Id, rclass, rtype, date, time, timezone, noMelee, sr);
            r.message = await ReplyAsync(r.Build());
            Adderbot.raids.Add(Context.Channel.Id, r);
            await Context.Message.DeleteAsync();
        }

        [Command("delete")]
        [Summary("Deletes raid associated with user.")]
        public async Task DeleteAsync()
        {
            List<ulong> tbd = new List<ulong>();
            var userRaids = Adderbot.raids.Where(x => x.Key == Context.Channel.Id && x.Value.lead == Context.User.Id);
            foreach (var r in userRaids)
            {
                await r.Value.message.DeleteAsync();
                tbd.Add(r.Key);
            }
            foreach (ulong deletee in tbd)
            {
                Adderbot.raids.Remove(deletee);
            }
            await Context.Message.DeleteAsync();
        }

        [Command("purge")]
        [Summary("Purges channel of all messages")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeAsync()
        {
            List<ulong> toBeDeleted = new List<ulong>();
            IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(100).FlattenAsync());
            foreach (var deletee in messages.Where(x => x.Author.IsBot))
            {
                foreach (var raid in Adderbot.raids)
                {
                    if (raid.Value.message.Id == deletee.Id)
                        toBeDeleted.Add(raid.Key);
                }
            }
            foreach (var deletee in toBeDeleted)
            {
                Adderbot.raids.Remove(deletee);
            }
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            IUserMessage m = await ReplyAsync($"Purge completed. _This message will be deleted in 5 seconds_");
            for (int i = 4; i > 0; i--)
            {
                await Task.Delay(1000);
                await m.ModifyAsync(x => x.Content = $"Purge completed. _This message will be deleted in {i} seconds_");
            }
            await m.DeleteAsync();
        }

        public static async Task UpdateRoster(SocketCommandContext scc, string role, string user)
        {
            Raid r = Adderbot.raids.GetValueOrDefault(scc.Channel.Id);
            if (((SocketGuildUser)scc.User).Roles.Intersect(r.restrictedRoles).Count() == 0)
            {
                await scc.User.SendMessageAsync($"You aren't allowed to join this raid, please message <@{r.lead}> for more information.");
            }
            else
            {
                r.AddPlayer($"<@{scc.User.Id.ToString() + (user ?? "")}>", role);
                await ((IUserMessage)r.message).ModifyAsync(x => x.Content = r.Build());
            }
            await scc.Channel.DeleteMessageAsync(scc.Message);
        }
    }
}