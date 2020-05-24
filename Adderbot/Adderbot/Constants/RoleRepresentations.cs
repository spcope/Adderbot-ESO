using System.Collections.Generic;
using Adderbot.Models;

namespace Adderbot.Constants
{
    public static class RoleRepresentations
    {
        public const string Mt = ":shield: **MT**";
        public const string Ot = ":scales: **OT**";
        public const string H1 = ":syringe: **H1**";
        public const string H2 = ":pill: **H2**";
        public const string Dps = ":monkey: **DPS**";
        public const string MDps = ":crossed_swords: **mDPS**";
        public const string RDps = ":comet: **rDPS**";
        public const string Cro = ":skull: **Cro**";
        public const string Gh = ":syringe: **GH**";
        public const string Th = ":syringe: **TH**";
        public const string Kh = ":pill: **KH**";
        public const string Ch = ":pill: **CH**";
        public const string AltHealer = ":eyes: **Alt Healer**";
        public const string AltTank = ":eyes: **Alt Tank**";
        public const string AltDps = ":eyes: **Alt DPS**";
        public const string AltMDps = ":eyes: **Alt mDPS**";
        public const string AltRDps = ":eyes: **Alt rDPS**";
        public const string AltNecro = ":skull_crossbones: **Alt Necro**";

        public static readonly Dictionary<Role, string> RoleToRepresentation = new Dictionary<Role, string>()
        {
            {Role.Mt, Mt}, {Role.Ot, Ot}, {Role.H1, H1}, {Role.H2, H2}, {Role.Ch, Ch},
            {Role.Dps, Dps}, {Role.MDps, MDps}, {Role.RDps, RDps}, {Role.Gh, Gh}, {Role.Th, Th},
            {Role.Kh, Kh}, {Role.AltHealer, AltHealer}, {Role.AltTank, AltTank},
            {Role.AltDps, AltDps}, {Role.AltMDps, AltMDps}, {Role.AltRDps, AltRDps}
        };
    }
}