using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Modules.Base;

namespace Adderbot.Modules.Roles
{
    public class HealerModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        [Command("h")]
        [Summary("Adds a user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HAsync([Remainder] string user = null) => await HealerAsync(user);


        [Command("healer")]
        [Summary("Adds a user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HealerAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.H);
        #endregion

        #region Healer Specifics
        #region H1
        [Command("healer1")]
        [Summary("Adds user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer1Async([Remainder] string user = null) => await H1Async(user);

        [Command("h1")]
        [Summary("Adds user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H1Async([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.H1);
        #endregion

        #region H2
        [Command("healer2")]
        [Summary("Adds user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer2Async([Remainder] string user = null) => await H2Async(user);

        [Command("h2")]
        [Summary("Adds user as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H2Async([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.H2);
        #endregion
        
        #region GH
        [Command("grouphealer")]
        [Summary("Adds a user as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GroupHealerAsync([Remainder] string user = null) => await GhAsync(user);


        [Command("gh")]
        [Summary("Adds a user as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GhAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.Gh);
        #endregion

        #region KH
        [Command("kitehealer")]
        [Summary("Adds a user as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KiteHealerAsync([Remainder] string user = null) => await KhAsync(user);

        [Command("kh")]
        [Summary("Adds a user as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KhAsync([Remainder] string user = null) => await BaseModule.UpdateRoster(Context, Role.Kh);
        #endregion
        #endregion
    }
}
