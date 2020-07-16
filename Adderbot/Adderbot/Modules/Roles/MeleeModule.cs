using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class MeleeModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        /// <summary>
        /// Handles ;m
        /// Signs the user up as a mDPS
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("m")]
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
        [Command("mdps")]
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
        [Command("stam")]
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
