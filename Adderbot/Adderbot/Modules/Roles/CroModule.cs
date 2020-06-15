using System.Threading.Tasks;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules.Roles
{
    internal class CroModule : BaseModule
    {
        #region Generics
        [Command("n")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task NAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }
        
        [Command("c")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        [Command("cro")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        [Command("magcro")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MagcroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }
        
        [Command("stamcro")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamcroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        [Command("necro")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task NecroAsync([Remainder] string emote = null)
        {
            await BaseModule.UpdateRoster(Context, Role.Cro, emote);
        }
        #endregion
    }
}