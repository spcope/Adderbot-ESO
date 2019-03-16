using System;
using System.Collections.Generic;
using System.Text;

namespace Adderbot.Models
{
    public static class RoleRepresentations
    {
        public static string MT = ":shield: **MT**";
        public static string OT = ":scales: **OT**";
        public static string OT2 = ":scales: **OT2**";
        public static string H1 = ":syringe: **H1**";
        public static string H2 = ":pill: **H2**";
        public static string DPS = ":monkey: **DPS**";
        public static string mDPS = ":crossed_swords: **mDPS**";
        public static string rDPS = ":comet: **rDPS**";
        public static string GH = ":syringe: **GH**";
        public static string KH = ":pill: **KH**";
        public static string Alt_Healer = ":eyes: **Alt Healer**";
        public static string Alt_Tank = ":eyes: **Alt Tank**";
        public static string Alt_DPS = ":eyes: **Alt DPS**";
        public static string Alt_mDPS = ":eyes: **Alt mDPS**";
        public static string Alt_rDPS = ":eyes: **Alt rDPS**";

        public static Dictionary<Role, string> RoleToRepresentation = new Dictionary<Role, string>()
        {
            { Role.MT, MT }, { Role.OT, OT }, { Role.OT2, OT2 }, { Role.H1, H1 }, { Role.H2, H2 },
            { Role.DPS, DPS }, { Role.mDPS, mDPS }, { Role.rDPS, rDPS }, { Role.GH, GH },
            { Role.KH, KH }, { Role.Alt_Healer, Alt_Healer }, { Role.Alt_Tank, Alt_Tank },
            { Role.Alt_DPS, Alt_DPS }, { Role.Alt_mDPS, Alt_mDPS }, { Role.Alt_rDPS, Alt_rDPS }
        };
    }
}
