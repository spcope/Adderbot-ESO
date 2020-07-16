using System.Collections.Generic;
using Adderbot.Models;

namespace Adderbot.Constants
{
    /// <summary>
    /// Contains representations of the various role types used by the bot
    /// </summary>
    public static class RoleRepresentations
    {
        private const string Mt = ":shield: **MT**";
        private const string Ot = ":scales: **OT**";
        private const string H1 = ":syringe: **H1**";
        private const string H2 = ":pill: **H2**";
        private const string Dps = ":monkey: **DPS**";
        private const string MDps = ":crossed_swords: **mDPS**";
        private const string RDps = ":comet: **rDPS**";
        private const string Cro = ":skull: **Cro**";
        private const string Gh = ":syringe: **GH**";
        private const string Th = ":syringe: **TH**";
        private const string Kh = ":pill: **KH**";
        private const string Ch = ":pill: **CH**";
        private const string AltHealer = ":eyes: **Alt Healer**";
        private const string AltTank = ":eyes: **Alt Tank**";
        private const string AltDps = ":eyes: **Alt DPS**";
        private const string AltMDps = ":eyes: **Alt mDPS**";
        private const string AltRDps = ":eyes: **Alt rDPS**";
        private const string AltCro = ":eyes: **Alt Necro**";

        /// <summary>
        /// Map of Role to its textual representation
        /// </summary>
        private static readonly Dictionary<Role, string> RoleToRepresentation = new Dictionary<Role, string>()
        {
            {Role.Mt, Mt}, {Role.Ot, Ot}, {Role.H1, H1}, {Role.H2, H2}, {Role.Ch, Ch},
            {Role.Dps, Dps}, {Role.MDps, MDps}, {Role.RDps, RDps}, {Role.Cro, Cro}, {Role.Gh, Gh}, {Role.Th, Th},
            {Role.Kh, Kh}, {Role.AltHealer, AltHealer}, {Role.AltTank, AltTank},
            {Role.AltDps, AltDps}, {Role.AltMDps, AltMDps}, {Role.AltRDps, AltRDps}, {Role.AltCro, AltCro}
        };

        /// <summary>
        /// Gets the representation for the passed in role
        /// </summary>
        /// <param name="role">Role to get the representation for</param>
        /// <returns>String representation of the role</returns>
        public static string GetRepresentation(Role role)
        {
            return RoleToRepresentation.GetValueOrDefault(role);
        }
    }
}