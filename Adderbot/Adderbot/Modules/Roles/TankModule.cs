using Adderbot.Models;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Adderbot.Constants;

namespace Adderbot.Modules.Roles
{
    public class TankModule : BaseModule
    {
        #region Generics
        /// <summary>
        /// Handles ;t
        /// Signs the user up as the T role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.TankRole.T)]
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
        [Command(Keywords.Role.TankRole.Tank)]
        [Summary("Adds a emote as a tank.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task TankAsync([Remainder] string emote = null) => await UpdateRoster(Role.T, emote);
        #endregion

        #region Tank Specifics
        #region MT
        /// <summary>
        /// Handles ;maintank
        /// Signs the user up as the MT role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.TankRole.MainTank)]
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
        [Command(Keywords.Role.TankRole.Mt)]
        [Summary("Adds a emote as a MT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task MtAsync([Remainder] string emote = null) => await UpdateRoster(Role.Mt, emote);
        #endregion

        #region OT
        /// <summary>
        /// Handles ;offtank
        /// Signs the user up as the OT role with the given emote
        /// </summary>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command(Keywords.Role.TankRole.OffTank)]
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
        [Command(Keywords.Role.TankRole.Ot)]
        [Summary("Adds a emote as an OT")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task OtAsync([Remainder] string emote = null) => await UpdateRoster(Role.Ot, emote);
        #endregion
        #endregion
    }
}
