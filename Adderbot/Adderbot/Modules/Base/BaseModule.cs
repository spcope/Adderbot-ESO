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

        [Command("create")]
        [Summary("Creates a raid.")]
        public async Task CreateAsync([Remainder] string raidInfo)
        {
            if (!((SocketGuildUser) Context.User).Roles.Contains(Adderbot.trialLead))
            {
                await Context.User.SendMessageAsync("You do not have permission to create raids!");
                await Context.Message.DeleteAsync();
                return;
            }
            string[] info = raidInfo.Split(' ');
            bool noMelee = false;
            if (info.Length == 6)
                noMelee = true;
            Raid r = new Raid(Context.User.Id, info[0], info[1], info[2], info[3], info[4], noMelee);
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
            Adderbot.raids.TryGetValue(scc.Channel.Id, out Raid r);
            r.AddPlayer($"<@{scc.User.Id.ToString() + (user ?? "")}>", role);
            await ((IUserMessage)r.message).ModifyAsync(x => x.Content = r.Build());
            await scc.Channel.DeleteMessageAsync(scc.Message);
        }
    }
}
