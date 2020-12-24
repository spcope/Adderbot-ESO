using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules.Roles
{
    /// <summary>
    /// Module of all commands related to signing up as a Necromancer
    /// </summary>
    public class CroModule : BaseModule
    {
        #region Generics

        /// <summary>
        /// Handles ;n
        /// Signs the user up as a necromancer with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.N)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task NAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        /// <summary>
        /// Handles ;c
        /// Signs the user up as a necromancer with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.C)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        /// <summary>
        /// Handles ;cro
        /// Signs the user up as a necromancer with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.Cro)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task CroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        /// <summary>
        /// Handles ;magcro
        /// Signs the user up as a necromancer with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.MagCro)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MagcroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        /// <summary>
        /// Handles ;stamcro
        /// Signs the user up as a necromancer with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.StamCro)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamcroAsync([Remainder] string emote = null)
        {
            await NecroAsync(emote);
        }

        /// <summary>
        /// Handles ;necro
        /// Signs the user up as a necromancer with the given emote
        /// All base necro commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.CroRole.Necro)]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task NecroAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Role.Cro, emote);
        }

        #endregion
    }
}