using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class MeleeModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("m")]
        [Summary("Adds user as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string user = null)
        {
            await MeleeAsync(user);
        }

        [Command("mdps")]
        [Summary("Adds user as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MdpsAsync([Remainder] string user = null)
        {
            await MeleeAsync(user);
        }

        [Command("stam")]
        [Summary("Adds user as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string user = null)
        {
            await MeleeAsync(user);
        }

        [Command("melee")]
        [Summary("Adds user as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MeleeAsync([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, "m", user);
        }
        #endregion

        #region Specific Spots

        #region M1
        [Command("melee1")]
        [Summary("Adds a player as melee dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Melee1Async([Remainder] string user = null)
        {
            await M1Async(user);
        }

        [Command("mdsp1")]
        [Summary("Adds a player as melee dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Mdps1Async([Remainder] string user = null)
        {
            await M1Async(user);
        }

        [Command("m1")]
        [Summary("Adds a player as melee dps slot 1")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task M1Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, "m1", user);
        }
        #endregion

        #region M2
        [Command("melee2")]
        [Summary("Adds a player as melee dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Melee2Async([Remainder] string user = null)
        {
            await M2Async(user);
        }

        [Command("mdsp2")]
        [Summary("Adds a player as melee dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Mdps2Async([Remainder] string user = null)
        {
            await M2Async(user);
        }

        [Command("m2")]
        [Summary("Adds a player as melee dps slot 2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task M2Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, "m2", user);
        }
        #endregion

        #region M3
        [Command("melee3")]
        [Summary("Adds a player as melee dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Melee3Async([Remainder] string user = null)
        {
            await M3Async(user);
        }

        [Command("mdsp3")]
        [Summary("Adds a player as melee dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Mdps3Async([Remainder] string user = null)
        {
            await M3Async(user);
        }

        [Command("m3")]
        [Summary("Adds a player as melee dps slot 3")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task M3Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, "m3", user);
        }
        #endregion

        #region M4
        [Command("melee4")]
        [Summary("Adds a player as melee dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Melee4Async([Remainder] string user = null)
        {
            await M4Async(user);
        }

        [Command("mdsp4")]
        [Summary("Adds a player as melee dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Mdps4Async([Remainder] string user = null)
        {
            await M4Async(user);
        }

        [Command("m4")]
        [Summary("Adds a player as melee dps slot 4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task M4Async([Remainder] string user = null)
        {
            await BaseModule.UpdateRoster(Context, "m4", user);
        }
        #endregion

        #endregion
    }
}
