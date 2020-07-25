using System;
using Adderbot.Constants;
using Adderbot.Models;

namespace Adderbot.Helpers
{
    /// <summary>
    /// Contains helper methods for the Role enum
    /// </summary>
    public static class RoleHelper
    {
        public static Role GetRoleFromString(string role)
        {
            switch (role)
            {
                case Keywords.Role.DpsRole.D:
                case Keywords.Role.DpsRole.Dd:
                case Keywords.Role.DpsRole.Dps:
                case Keywords.Role.DpsRole.Damage:
                    return Role.Dps;
                case Keywords.Role.RangedDamageRole.R:
                case Keywords.Role.RangedDamageRole.Range:
                case Keywords.Role.RangedDamageRole.RDps:
                case Keywords.Role.RangedDamageRole.Ranged:
                    return Role.RDps;
                case Keywords.Role.MeleeDamageRole.M:
                case Keywords.Role.MeleeDamageRole.Melee:
                case Keywords.Role.MeleeDamageRole.MDps:
                    return Role.MDps;
                case Keywords.Role.CroRole.C:
                case Keywords.Role.CroRole.N:
                case Keywords.Role.CroRole.Necro:
                case Keywords.Role.CroRole.Cro:
                case Keywords.Role.CroRole.MagCro:
                case Keywords.Role.CroRole.StamCro:
                    return Role.Cro;
                case Keywords.Role.TankRole.T:
                case Keywords.Role.TankRole.Tank:
                    return Role.T;
                case Keywords.Role.TankRole.Mt:
                case Keywords.Role.TankRole.MainTank:
                    return Role.Mt;
                case Keywords.Role.HealerRole.H:
                case Keywords.Role.HealerRole.Healer:
                    return Role.H;
                case Keywords.Role.HealerRole.H1:
                case Keywords.Role.HealerRole.Healer1:
                    return Role.H1;
                case Keywords.Role.HealerRole.H2:
                case Keywords.Role.HealerRole.Healer2:
                    return Role.H2;
                case Keywords.Role.HealerRole.Gh:
                case Keywords.Role.HealerRole.GroupHealer:
                    return Role.Gh;
                case Keywords.Role.HealerRole.Th:
                case Keywords.Role.HealerRole.TankHealer:
                    return Role.Th;
                case Keywords.Role.HealerRole.Kh:
                case Keywords.Role.HealerRole.KiteHealer:
                    return Role.Kh;
                case Keywords.Role.AltRole.AltCro:
                    return Role.AltCro;
                case Keywords.Role.AltRole.AltDps:
                    return Role.AltDps;
                case Keywords.Role.AltRole.AltMDps:
                    return Role.AltMDps;
                case Keywords.Role.AltRole.AltRDps:
                    return Role.AltRDps;
                case Keywords.Role.AltRole.AltHealer:
                    return Role.AltHealer;
                case Keywords.Role.AltRole.AltTank:
                    return Role.AltTank;
                default:
                    return Role.InvalidRole;
            }
        }
    }
}