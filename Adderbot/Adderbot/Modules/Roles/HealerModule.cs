using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class HealerModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("h")]
        [Summary("Adds a emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HAsync([Remainder] string emote = null) => await HealerAsync(emote);


        [Command("healer")]
        [Summary("Adds a emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HealerAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H, emote);
        #endregion

        #region Healer Specifics
        #region H1
        [Command("healer1")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer1Async([Remainder] string emote = null) => await H1Async(emote);

        [Command("h1")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H1Async([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H1, emote);
        #endregion

        #region H2
        [Command("healer2")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer2Async([Remainder] string emote = null) => await H2Async(emote);

        [Command("h2")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H2Async([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H2, emote);
        #endregion
        
        #region GH
        [Command("grouphealer")]
        [Summary("Adds a emote as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GroupHealerAsync([Remainder] string emote = null) => await GhAsync(emote);


        [Command("gh")]
        [Summary("Adds a emote as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GhAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Gh, emote);
        #endregion

        #region KH
        [Command("kitehealer")]
        [Summary("Adds a emote as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KiteHealerAsync([Remainder] string emote = null) => await KhAsync(emote);

        [Command("kh")]
        [Summary("Adds a emote as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KhAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Kh, emote);
        #endregion
        
        #region CH
        [Command("cagehealer")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CageHealerAsync([Remainder] string emote = null) => await ChAsync(emote);

        [Command("ch")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ChAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Ch, emote);
        #endregion
        
        #region TH
        [Command("tankhealer")]
        [Summary("Adds emote as a tank healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankHealerAsync([Remainder] string emote = null) => await ThAsync(emote);

        [Command("th")]
        [Summary("Adds emote as a tank healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ThAsync([Remainder] string emote = null) =>
            await BaseModule.UpdateRoster(Context, Role.Th, emote);
        #endregion

        #endregion
    }
}
