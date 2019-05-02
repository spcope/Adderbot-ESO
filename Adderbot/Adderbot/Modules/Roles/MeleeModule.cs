using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Modules.Base;

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
            await BaseModule.UpdateRoster(Context, Role.MDps);
        }
        #endregion
    }
}
