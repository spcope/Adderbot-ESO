using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules.Roles
{
    public class DpsModule : BaseModule
    {
        #region Generics

        /// <summary>
        /// Handles ;d
        /// Adds the user as a DPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.DpsRole.D)]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DAsync([Remainder] string emote = null)
        {
            await DpsAsync(emote);
        }

        /// <summary>
        /// Handles ;dd
        /// Adds the user as a DPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("dd")]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DdAsync([Remainder] string emote = null)
        {
            await DpsAsync(emote);
        }

        /// <summary>
        /// Handles ;damage
        /// Adds the user as a DPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("damage")]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DamageAsync([Remainder] string emote = null)
        {
            await DpsAsync(emote);
        }

        /// <summary>
        /// Handles ;dps
        /// Adds the user as a DPS
        /// All dps commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("dps")]
        [Summary("Adds user as a dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DpsAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Role.Dps, emote);
        }

        #endregion
    }
}