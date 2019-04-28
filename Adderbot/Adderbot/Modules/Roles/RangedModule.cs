using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class RangedModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("r")]
        [Summary("Adds user as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string user = null)
        {
            await RangedAsync(user);
        }

        [Command("rdps")]
        [Summary("Adds user as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RdpsAsync([Remainder] string user = null)
        {
            await RangedAsync(user);
        }

        [Command("range")]
        [Summary("Adds user as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string user = null)
        {
            await RangedAsync(user);
        }

        [Command("ranged")]
        [Summary("Adds user as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RangedAsync([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps, user);
        }
        #endregion

        #region Specific Spots

        #region R1
        [Command("ranged1")]
        [Summary("Adds a player as ranged dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Ranged1Async([Remainder] string user = null)
        {
            await R1Async(user);
        }

        [Command("rdps1")]
        [Summary("Adds a player as ranged dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Rdps1Async([Remainder] string user = null)
        {
            await R1Async(user);
        }

        [Command("r1")]
        [Summary("Adds a player as ranged dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task R1Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps1, user);
        }
        #endregion

        #region R2
        [Command("ranged2")]
        [Summary("Adds a player as ranged dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Ranged2Async([Remainder] string user = null)
        {
            await R2Async(user);
        }

        [Command("rdps2")]
        [Summary("Adds a player as ranged dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Rdps2Async([Remainder] string user = null)
        {
            await R2Async(user);
        }

        [Command("r2")]
        [Summary("Adds a player as ranged dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task R2Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps2, user);
        }
        #endregion

        #region R3
        [Command("ranged3")]
        [Summary("Adds a player as ranged dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Ranged3Async([Remainder] string user = null)
        {
            await R3Async(user);
        }

        [Command("rdps3")]
        [Summary("Adds a player as ranged dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Rdps3Async([Remainder] string user = null)
        {
            await R3Async(user);
        }

        [Command("r3")]
        [Summary("Adds a player as ranged dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task R3Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps3, user);
        }
        #endregion

        #region R4
        [Command("ranged4")]
        [Summary("Adds a player as ranged dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Ranged4Async([Remainder] string user = null)
        {
            await R4Async(user);
        }

        [Command("rdps4")]
        [Summary("Adds a player as ranged dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Rdps4Async([Remainder] string user = null)
        {
            await R4Async(user);
        }

        [Command("r4")]
        [Summary("Adds a player as ranged dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task R4Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps4, user);
        }
        #endregion

        #endregion
    }
}
