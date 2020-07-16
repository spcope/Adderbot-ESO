using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    /// <summary>
    /// Module of all commands related to signing up as a Healer
    /// </summary>
    public class HealerModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        /// <summary>
        /// Handles ;h
        /// Signs the user up as a generic healer
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("h")]
        [Summary("Adds a emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HAsync([Remainder] string emote = null) => await HealerAsync(emote);

        /// <summary>
        /// Handles ;healer
        /// Signs the user up as a generic healer
        /// All generic healer commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("healer")]
        [Summary("Adds a emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HealerAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H, emote);
        #endregion

        #region Healer Specifics
        #region H1
        /// <summary>
        /// Handles ;healer1
        /// Signs the user up as the H1 role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("healer1")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer1Async([Remainder] string emote = null) => await H1Async(emote);

        /// <summary>
        /// Handles ;h1
        /// Signs the user up as the H1 role with the given emote
        /// All H1 commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("h1")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H1Async([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H1, emote);
        #endregion

        #region H2
        /// <summary>
        /// Handles ;healer2
        /// Signs the user up as the H2 role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("healer2")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Healer2Async([Remainder] string emote = null) => await H2Async(emote);

        /// <summary>
        /// Handles ;h2
        /// Signs the user up as the H2 role with the given emote
        /// All H2 commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("h2")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H2Async([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.H2, emote);
        #endregion
        
        #region GH
        /// <summary>
        /// Handles ;grouphealer
        /// Signs the user up as the GH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("grouphealer")]
        [Summary("Adds a emote as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GroupHealerAsync([Remainder] string emote = null) => await GhAsync(emote);

        /// <summary>
        /// Handles ;gh
        /// Signs the user up as the GH role with the given emote
        /// All GH commands route to this method
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        [Command("gh")]
        [Summary("Adds a emote as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GhAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Gh, emote);
        #endregion

        #region KH
        /// <summary>
        /// Handles ;kitehealer
        /// Signs the user up as the KH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("kitehealer")]
        [Summary("Adds a emote as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KiteHealerAsync([Remainder] string emote = null) => await KhAsync(emote);

        /// <summary>
        /// Handles ;kh
        /// Signs the user up as the KH role with the given emote
        /// All KH commands route to this method
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        [Command("kh")]
        [Summary("Adds a emote as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KhAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Kh, emote);
        #endregion
        
        #region CH
        /// <summary>
        /// Handles ;cagehealer
        /// Signs the user up as the CH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("cagehealer")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CageHealerAsync([Remainder] string emote = null) => await ChAsync(emote);

        /// <summary>
        /// Handles ;ch
        /// Signs the user up as the CH role with the given emote
        /// All CH commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("ch")]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ChAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Ch, emote);
        #endregion
        
        #region TH
        /// <summary>
        /// Handles ;tankhealer
        /// Signs the user up as the TH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("tankhealer")]
        [Summary("Adds emote as a tank healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankHealerAsync([Remainder] string emote = null) => await ThAsync(emote);

        /// <summary>
        /// Handles ;th
        /// Signs the user up as the TH role with the given emote
        /// All TH commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("th")]
        [Summary("Adds emote as a tank healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ThAsync([Remainder] string emote = null) =>
            await BaseModule.UpdateRoster(Context, Role.Th, emote);
        #endregion

        #endregion
    }
}
