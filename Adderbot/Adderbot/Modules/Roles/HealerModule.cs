using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Constants;

namespace Adderbot.Modules.Roles
{
    /// <summary>
    /// Module of all commands related to signing up as a Healer
    /// </summary>
    public class HealerModule : BaseModule
    {
        #region Generics

        /// <summary>
        /// Handles ;h
        /// Signs the user up as a generic healer
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.H)]
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
        [Command(Keywords.Role.HealerRole.Healer)]
        [Summary("Adds a emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task HealerAsync([Remainder] string emote = null) => await UpdateRoster(Role.H, emote);

        #endregion

        #region Healer Specifics

        #region H1

        /// <summary>
        /// Handles ;healer1
        /// Signs the user up as the H1 role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.Healer1)]
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
        [Command(Keywords.Role.HealerRole.H1)]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H1Async([Remainder] string emote = null) => await UpdateRoster(Role.H1, emote);

        #endregion

        #region H2

        /// <summary>
        /// Handles ;healer2
        /// Signs the user up as the H2 role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.Healer2)]
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
        [Command(Keywords.Role.HealerRole.H2)]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task H2Async([Remainder] string emote = null) => await UpdateRoster(Role.H2, emote);

        #endregion

        #region GH

        /// <summary>
        /// Handles ;grouphealer
        /// Signs the user up as the GH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.GroupHealer)]
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
        [Command(Keywords.Role.HealerRole.Gh)]
        [Summary("Adds a emote as a group healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task GhAsync([Remainder] string emote = null) => await UpdateRoster(Role.Gh, emote);

        #endregion

        #region KH

        /// <summary>
        /// Handles ;kitehealer
        /// Signs the user up as the KH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.KiteHealer)]
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
        [Command(Keywords.Role.HealerRole.Kh)]
        [Summary("Adds a emote as a kite healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task KhAsync([Remainder] string emote = null) => await UpdateRoster(Role.Kh, emote);

        #endregion

        #region CH

        /// <summary>
        /// Handles ;cagehealer
        /// Signs the user up as the CH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.CageHealer)]
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
        [Command(Keywords.Role.HealerRole.Ch)]
        [Summary("Adds emote as a healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ChAsync([Remainder] string emote = null) => await UpdateRoster(Role.Ch, emote);

        #endregion

        #region TH

        /// <summary>
        /// Handles ;tankhealer
        /// Signs the user up as the TH role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.HealerRole.TankHealer)]
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
        [Command(Keywords.Role.HealerRole.Th)]
        [Summary("Adds emote as a tank healer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ThAsync([Remainder] string emote = null) =>
            await UpdateRoster(Role.Th, emote);

        #endregion

        #endregion
    }
}