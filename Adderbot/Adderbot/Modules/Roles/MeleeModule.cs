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
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        [Command("mdps")]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MdpsAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        [Command("stam")]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        [Command("melee")]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MeleeAsync([Remainder] string emote = null)
        {
            await BaseModule.UpdateRoster(Context, Role.MDps, emote);
        }
        #endregion
    }
}
