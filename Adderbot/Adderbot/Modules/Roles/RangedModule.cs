using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Constants;

namespace Adderbot.Modules.Roles
{
    public class RangedModule : BaseModule
    {
        #region Generics
        /// <summary>
        /// Handles ;r
        /// Signs the user up as the rDPS role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.RangedDamageRole.R)]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        /// <summary>
        /// Handles ;rdps
        /// Signs the user up as the rDPS role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.RangedDamageRole.RDps)]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RdpsAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        /// <summary>
        /// Handles ;range
        /// Signs the user up as the rDPS role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.RangedDamageRole.Range)]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string emote = null)
        {
            await RangedAsync(emote);
        }

        /// <summary>
        /// Handles ;ranged
        /// Signs the user up as the rDPS role with the given emote
        /// All rDPS commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.RangedDamageRole.Ranged)]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RangedAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Role.RDps, emote);
        }
        #endregion
    }
}
