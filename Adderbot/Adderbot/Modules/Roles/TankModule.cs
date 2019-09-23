using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Modules.Base;

namespace Adderbot.Modules.Roles
{
    public class TankModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("t")]
        [Summary("Adds a emote as a tank")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAbbrevAsync([Remainder] string emote = null) => await TankAsync(emote);

        [Command("tank")]
        [Summary("Adds a emote as a tank.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.T, emote);
        #endregion

        #region Tank Specifics
        #region MT
        [Command("maintank")]
        [Summary("Adds a emote as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MainTankAsync([Remainder] string emote = null) => await MtAsync(emote);

        [Command("mt")]
        [Summary("Adds a emote as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MtAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Mt, emote);
        #endregion

        #region OT
        [Command("offtank")]
        [Summary("Adds a emote as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OffTankAsync([Remainder] string emote = null) => await OtAsync(emote);

        [Command("ot")]
        [Summary("Adds a emote as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OtAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Ot, emote);
        #endregion
        #endregion
    }
}
