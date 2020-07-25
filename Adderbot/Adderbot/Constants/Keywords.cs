namespace Adderbot.Constants
{
    /// <summary>
    /// Contains keywords in commands (those that are specific)
    /// </summary>
    public class Keywords
    {
        /// <summary>
        /// Contains keywords in the Create script
        /// </summary>
        public class Create
        {
            public const string Aa = "aa";
            public const string So = "so";
            public const string Hrc = "hrc";
            public const string Mol = "mol";
            public const string Hof = "hof";
            public const string Cr0 = "cr+0";
            public const string Cr1 = "cr+1";
            public const string Cr2 = "cr+2";
            public const string Cr3 = "cr+3";
            public const string As0 = "as+0";
            public const string As1 = "as+1";
            public const string As2 = "as+2";
            public const string Ss = "ss";
            public const string SsI = "ss+i";
            public const string SsF = "ss+f";
            public const string SsFI = "ss+fi";
            public const string Ka = "ka";
            public const string KaV = "ka+v";
            public const string KaY = "ka+y";
            public const string KaVY = "ka+vy";

            public const string V = "v";
            public const string Vet = "vet";
            public const string Veteran = "veteran";
            public const string HM = "hm";
            public const string Hardmode = "hardmode";
            public const string N = "n";
            public const string Norm = "norm";
            public const string Normal = "normal";
        }

        /// <summary>
        /// Contains keywords related to roles in commands
        /// </summary>
        public class Role
        {
            /// <summary>
            /// Contains keywords related to tanks in commands
            /// </summary>
            public class TankRole
            {
                public const string T = "t";
                public const string Tank = "tank";
                public const string Mt = "mt";
                public const string MainTank = "maintank";
                public const string Ot = "ot";
                public const string OffTank = "offtank";
            }

            /// <summary>
            /// Contains keywords related to healers in commands
            /// </summary>
            public class HealerRole
            {
                public const string H = "h";
                public const string Healer = "healer";
                public const string H1 = "h1";
                public const string Healer1 = "healer1";
                public const string H2 = "h2";
                public const string Healer2 = "healer2";
                public const string Gh = "gh";
                public const string GroupHealer = "grouphealer";
                public const string Ch = "ch";
                public const string CageHealer = "cagehealer";
                public const string Kh = "kh";
                public const string KiteHealer = "kitehealer";
                public const string Th = "th";
                public const string TankHealer = "tankhealer";
            }

            /// <summary>
            /// Contains keywords related to DPS in commands
            /// </summary>
            public class DpsRole
            {
                public const string D = "d";
                public const string Dd = "dd";
                public const string Dps = "dps";
                public const string Damage = "damage";
            }

            /// <summary>
            /// Contains keywords related to ranged DPS in commands
            /// </summary>
            public class RangedDamageRole
            {
                public const string R = "r";
                public const string Range = "range";
                public const string Ranged = "ranged";
                public const string RDps = "rdps";
            }

            /// <summary>
            /// Contains keywords related to melee DPS in commands
            /// </summary>
            public class MeleeDamageRole
            {
                public const string M = "m";
                public const string Melee = "melee";
                public const string Stam = "stam";
                public const string MDps = "mdps";
            }

            /// <summary>
            /// Contains keywords related to cro DPS in commands
            /// </summary>
            public class CroRole
            {
                public const string C = "c";
                public const string Cro = "cro";
                public const string N = "n";
                public const string Necro = "necro";
                public const string MagCro = "magcro";
                public const string StamCro = "stamcro";
            }

            /// <summary>
            /// Contains keywords related to alt commands
            /// </summary>
            public class AltRole
            {
                public const string AltDps = "alt-dps";
                public const string AltMDps = "alt-mdps";
                public const string AltRDps = "alt-rdps";
                public const string AltCro = "alt-cro";
                public const string AltTank = "alt-tank";
                public const string AltHealer = "alt-healer";
            }
        }
    }
}