using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Adderbot.Modules.Roles
{
    public class TankModule : ModuleBase<SocketCommandContext>
    {
        #region Generics
        /// <summary>
        /// Handles ;t
        /// Signs the user up as the T role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("t")]
        [Summary("Adds a emote as a tank")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAbbrevAsync([Remainder] string emote = null) => await TankAsync(emote);

        /// <summary>
        /// Handles ;tank
        /// Signs the user up as the T role with the given emote
        /// All T commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("tank")]
        [Summary("Adds a emote as a tank.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.T, emote);
        #endregion

        #region Tank Specifics
        #region MT
        /// <summary>
        /// Handles ;maintank
        /// Signs the user up as the MT role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("maintank")]
        [Summary("Adds a emote as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MainTankAsync([Remainder] string emote = null) => await MtAsync(emote);

        /// <summary>
        /// Handles ;mt
        /// Signs the user up as the MT role with the given emote
        /// All MT commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("mt")]
        [Summary("Adds a emote as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MtAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Mt, emote);
        #endregion

        #region OT
        /// <summary>
        /// Handles ;offtank
        /// Signs the user up as the OT role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("offtank")]
        [Summary("Adds a emote as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OffTankAsync([Remainder] string emote = null) => await OtAsync(emote);

        /// <summary>
        /// Handles ;ot
        /// Signs the user up as the OT role with the given emote
        /// All OT commands route to this method
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("ot")]
        [Summary("Adds a emote as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OtAsync([Remainder] string emote = null) => await BaseModule.UpdateRoster(Context, Role.Ot, emote);
        #endregion
        #endregion
    }
}
