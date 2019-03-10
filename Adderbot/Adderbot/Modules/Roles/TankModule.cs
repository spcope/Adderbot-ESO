using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class TankModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("t")]
        [Summary("Adds a user as a tank")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TAsync([Remainder] string user = null) => await TankAsync(user);

        [Command("tank")]
        [Summary("Adds a user as a tank.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.T, user);
        #endregion

        #region Tank Specifics
        #region MT
        [Command("maintank")]
        [Summary("Adds a user as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MainTankAsync([Remainder] string user = null) => await MTAsync(user);

        [Command("mt")]
        [Summary("Adds a user as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MTAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.MT, user);
        #endregion

        #region OT
        [Command("offtank")]
        [Summary("Adds a user as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OffTankAsync([Remainder] string user = null) => await OTAsync(user);

        [Command("ot")]
        [Summary("Adds a user as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OTAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.OT, user);

        [Command("offtank2")]
        [Summary("Adds a user as an OT2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OffTank2Async([Remainder] string user = null) => await OT2Async(user);

        [Command("ot2")]
        [Summary("Adds a user as an OT2")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OT2Async([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.OT2, user);
        #endregion
        #endregion
    }
}
