using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Constants;

namespace Adderbot.Modules.Roles
{
    public class MeleeModule : BaseModule
    {
        #region Generics
        /// <summary>
        /// Handles ;m
        /// Signs the user up as a mDPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.MeleeDamageRole.M)]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        /// <summary>
        /// Handles ;mdps
        /// Signs the user up as a mDPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.MeleeDamageRole.MDps)]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MdpsAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        /// <summary>
        /// Handles ;stam
        /// Signs the user up as a mDPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.MeleeDamageRole.Stam)]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task StamAsync([Remainder] string emote = null)
        {
            await MeleeAsync(emote);
        }

        /// <summary>
        /// Handles ;melee
        /// Signs the user up as a mDPS
        /// All mDPS commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.MeleeDamageRole.Melee)]
        [Summary("Adds emote as a melee dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MeleeAsync([Remainder] string emote = null)
        {
            await UpdateRoster(Role.MDps, emote);
        }
        #endregion
    }
}
