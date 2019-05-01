using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Modules.Base;

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
    }
}
