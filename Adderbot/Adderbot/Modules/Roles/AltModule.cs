using System.Threading.Tasks;
using Adderbot.Constants;
using Adderbot.Helpers;
using Adderbot.Models;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules.Roles
{
    public class AltModule : BaseModule
    {
        /// <summary>
        /// Handles ;alt [role] [emote]
        /// Signs the user up as an alt [role] with the given emote (optional)
        /// </summary>
        /// <param name="role">String representation of the role</param>
        /// <param name="emote">String representation of the emote</param>
        /// <returns></returns>
        [Command("alt")]
        [Summary("Adds emote as a necro dps")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task AltAsync(string role, [Remainder] string emote = null)
        {
            var parsedRole = RoleHelper.GetRoleFromString(role);
            if (parsedRole == Role.InvalidRole) await SendMessageToUser(MessageText.Error.InvalidRole);
            else
            {
                switch (parsedRole)
                {
                    case Role.Mt:
                    case Role.Ot:
                    case Role.T:
                    case Role.AltTank:
                        await UpdateRoster(Role.AltTank, emote);
                        break;
                    case Role.H:
                    case Role.H1:
                    case Role.H2:
                    case Role.Ch:
                    case Role.Th:
                    case Role.AltHealer:
                        await UpdateRoster(Role.AltHealer, emote);
                        break;
                    case Role.RDps:
                    case Role.AltRDps:
                        await UpdateRoster(Role.AltRDps, emote);
                        break;
                    case Role.MDps:
                    case Role.AltMDps:
                        await UpdateRoster(Role.AltMDps, emote);
                        break;
                    case Role.Dps:
                    case Role.AltDps:
                        await UpdateRoster(Role.AltDps, emote);
                        break;
                    case Role.Cro:
                    case Role.AltCro:
                        await UpdateRoster(Role.AltCro, emote);
                        break;
                }
            }
        }
    }
}