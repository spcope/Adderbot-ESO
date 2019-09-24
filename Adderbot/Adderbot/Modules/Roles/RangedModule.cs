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
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        [Command("rdps")]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RdpsAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        [Command("range")]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        [Command("ranged")]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RangedAsync([Remainder] string emote = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps, emote);
        }
        #endregion
    }
}
