using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class RangedModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        /// <summary>
        /// Handles ;r
        /// Signs the user up as the rDPS role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("r")]
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
        [Command("rdps")]
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
        [Command("range")]
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
        [Command("ranged")]
        [Summary("Adds emote as a ranged dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task RangedAsync([Remainder] string emote = null)
        {
            await BaseModule.UpdateRoster(Context, Role.RDps, emote);
        }
        #endregion
    }
}
