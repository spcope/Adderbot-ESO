using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Adderbot.Models
{
    public enum Role
    {
        T, MT, OT, OT2, H, H1, H2, GH, KH, rDPS, rDPS1, rDPS2, rDPS3, rDPS4,
        mDPS, mDPS1, mDPS2, mDPS3, mDPS4, Alt_mDPS, Alt_rDPS, Alt_Tank,
        Alt_Healer, InvalidRole
    }

    public class Raid
    {
        private List<Role> availableRoles;
        public ulong lead;
        public List<SocketRole> restrictedRoles;
        public IMessage message;
        public string headline;
        public Dictionary<string, Role> currentPlayers;
        public string type;
        private readonly bool ignoreRoleType = false;
        private readonly bool noMelee = false;

        public Raid(ulong lead, string rclass, string type, string date, string time, string timezone, bool noMelee, SocketRole progRole)
        {
            restrictedRoles = new List<SocketRole>() { progRole };
            this.lead = lead;
            string RClass = "";
            switch (rclass.ToLower())
            {
                case "n":
                case "norm":
                case "normal":
                    RClass = "n";
                    break;
                case "v":
                case "vet":
                case "veteran":
                    RClass = "v";
                    break;
            }

            switch (type.ToLower())
            {
                #region Full Trials
                case "aa":
                    headline = $"Gear up for a {RClass}AA on {date} @{time} {timezone}";
                    availableRoles = new List<Role>() {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    ignoreRoleType = true;
                    this.type = "aa";
                    break;
                case "hrc":
                    headline = $"Gear up for a {RClass}HRC on {date} @{time} {timezone}";
                    availableRoles = new List<Role>() {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    this.type = "hrc";
                    break;
                case "mol":
                    headline = $"Gear up for a {RClass}MoL on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    this.type = "mol";
                    break;
                case "hof":
                    headline = $"Gear up for a {RClass}HoF on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    this.type = "hof";
                    break;
                #endregion

                #region Mini Trials
                case "cr+0":
                    headline = $"Gear up for a {RClass}CR+0 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "cr+0";
                    break;
                case "cr+1":
                    headline = $"Gear up for a {RClass}CR+1 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "cr+1";
                    break;
                case "cr+2":
                    headline = $"Gear up for a {RClass}CR+2 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "cr+2";
                    break;
                case "cr+3":
                    headline = $"Gear up for a {RClass}CR+3 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "cr+3";
                    break;
                case "as+0":
                    headline = $"Gear up for a {RClass}AS+0 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "as+0";
                    if (RClass.Equals("v"))
                        noMelee = true;
                    break;
                case "as+1":
                    headline = $"Gear up for a {RClass}AS+1 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "as+1";
                    if (RClass.Equals("v"))
                        noMelee = true;
                    break;
                case "as+2":
                    headline = $"Gear up for a {RClass}AS+2 on {date} @{time} {timezone}";
                    availableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    this.type = "as+2";
                    if (RClass.Equals("v"))
                        noMelee = true;
                    break;
                    #endregion
            }
            currentPlayers = new Dictionary<string, Role>();
            this.noMelee = noMelee;
        }

        public string Build()
        {
            string result = "";
            foreach(var role in restrictedRoles)
            {
                result += $"{role.Mention} ";
            }
            result += $"\n{headline}\n\n";
            switch (type)
            {
                #region Full Trials
                case "aa":
                    result += GenerateRole("MT", Role.MT) + GenerateRole("OT", Role.OT)
                        + GenerateRole("H1", Role.H1) + GenerateRole("H2", Role.H2)
                        + GenerateRole("DPS", Role.mDPS1) + GenerateRole("DPS", Role.mDPS2)
                        + GenerateRole("DPS", Role.mDPS3) + GenerateRole("DPS", Role.mDPS4)
                        + GenerateRole("DPS", Role.rDPS1) + GenerateRole("DPS", Role.rDPS2)
                        + GenerateRole("DPS", Role.rDPS3) + GenerateRole("DPS", Role.rDPS4)
                        + GenerateAlts();
                    break;
                case "hof":
                case "mol":
                case "hrc":
                    result += GenerateRole("MT", Role.MT) + GenerateRole("OT", Role.OT)
                        + GenerateRole("H1", Role.H1) + GenerateRole("H2", Role.H2)
                        + GenerateRole("mDPS 1", Role.mDPS1) + GenerateRole("mDPS 2", Role.mDPS2)
                        + GenerateRole("mDPS 3", Role.mDPS3) + GenerateRole("mDPS 4", Role.mDPS4)
                        + GenerateRole("rDPS 1", Role.rDPS1) + GenerateRole("rDPS 2", Role.rDPS2)
                        + GenerateRole("rDPS 3", Role.rDPS3) + GenerateRole("rDPS 4", Role.rDPS4)
                        + GenerateAlts();
                    break;
                #endregion

                #region Mini Trials
                case "cr+0":
                case "cr+1":
                case "cr+2":
                case "cr+3":
                    result += GenerateRole("MT", Role.MT) + GenerateRole("OT", Role.OT)
                        + GenerateRole("OT 2", Role.OT2) + GenerateRole("H1", Role.H1) 
                        + GenerateRole("H2", Role.H2);
                    if (noMelee)
                    {
                        result += GenerateRole("rDPS 1", Role.mDPS1) + GenerateRole("rDPS 2", Role.mDPS2)
                            + GenerateRole("rDPS 3", Role.mDPS3) + GenerateRole("rDPS 4", Role.mDPS4)
                            + GenerateRole("rDPS 5", Role.rDPS1) + GenerateRole("rDPS 6", Role.rDPS2)
                            + GenerateRole("rDPS 7", Role.rDPS3);
                    }
                    else
                    {
                        result += GenerateRole("DPS 1", Role.mDPS1) + GenerateRole("DPS 2", Role.mDPS2)
                           + GenerateRole("DPS 3", Role.mDPS3) + GenerateRole("DPS 4", Role.mDPS4)
                           + GenerateRole("DPS 5", Role.rDPS1) + GenerateRole("DPS 6", Role.rDPS2)
                           + GenerateRole("DPS 7", Role.rDPS3);
                    }
                    result += GenerateAlts();
                    break;
                case "as+0":
                case "as+1":
                case "as+2":
                    result += GenerateRole("MT", Role.MT) + GenerateRole("OT", Role.OT)
                        + GenerateRole("H1", Role.H1) + GenerateRole("H2", Role.H2);
                    if (noMelee)
                    {
                        result += GenerateRole("rDPS 1", Role.mDPS1) + GenerateRole("rDPS 2", Role.mDPS2)
                            + GenerateRole("rDPS 3", Role.mDPS3) + GenerateRole("rDPS 4", Role.mDPS4)
                            + GenerateRole("rDPS 5", Role.rDPS1) + GenerateRole("rDPS 6", Role.rDPS2)
                            + GenerateRole("rDPS 7", Role.rDPS3) + GenerateRole("rDPS 8", Role.rDPS4);
                    }
                    else
                    {
                        result += GenerateRole("DPS 1", Role.mDPS1) + GenerateRole("DPS 2", Role.mDPS2)
                           + GenerateRole("DPS 3", Role.mDPS3) + GenerateRole("DPS 4", Role.mDPS4)
                           + GenerateRole("DPS 5", Role.rDPS1) + GenerateRole("DPS 6", Role.rDPS2)
                           + GenerateRole("DPS 7", Role.rDPS3) + GenerateRole("DPS 8", Role.rDPS4);
                    }
                    result += GenerateAlts();
                    break;
                #endregion
            }
            return result;
        }

        public bool AddPlayer(string user, string role)
        {
            Role r = GetRole(role);
            if (r == Role.InvalidRole) return false;
            switch (r)
            {
                #region Generics
                case Role.mDPS:
                    if (!noMelee)
                    {
                        if (!currentPlayers.ContainsValue(Role.mDPS1))
                        {
                            currentPlayers.Add(user, Role.mDPS1);
                            break;
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS2))
                        {
                            currentPlayers.Add(user, Role.mDPS2);
                            break;
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS3))
                        {
                            currentPlayers.Add(user, Role.mDPS3);
                            break;
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS4))
                        {
                            currentPlayers.Add(user, Role.mDPS4);
                            break;
                        }
                        else if (!ignoreRoleType)
                        {
                            currentPlayers.Add(user, Role.Alt_mDPS);
                            break;
                        }

                        if (ignoreRoleType)
                        {
                            if (!currentPlayers.ContainsValue(Role.rDPS1))
                            {
                                currentPlayers.Add(user, Role.rDPS1);
                            }
                            else if (!currentPlayers.ContainsValue(Role.rDPS2))
                            {
                                currentPlayers.Add(user, Role.rDPS2);
                            }
                            else if (!currentPlayers.ContainsValue(Role.rDPS3))
                            {
                                currentPlayers.Add(user, Role.rDPS3);
                            }
                            else if (!currentPlayers.ContainsValue(Role.rDPS4))
                            {
                                currentPlayers.Add(user, Role.rDPS4);
                            }
                            else
                            {
                                currentPlayers.Add(user, Role.Alt_rDPS);
                            }
                        }
                    }
                    break;
                case Role.rDPS:
                    if (!currentPlayers.ContainsValue(Role.rDPS1))
                    {
                        currentPlayers.Add(user, Role.rDPS1);
                        break;
                    }
                    else if (!currentPlayers.ContainsValue(Role.rDPS2))
                    {
                        currentPlayers.Add(user, Role.rDPS2);
                        break;
                    }
                    else if (!currentPlayers.ContainsValue(Role.rDPS3))
                    {
                        currentPlayers.Add(user, Role.rDPS3);
                        break;
                    }
                    else if (!currentPlayers.ContainsValue(Role.rDPS4))
                    {
                        currentPlayers.Add(user, Role.rDPS4);
                        break;
                    }
                    else if (!ignoreRoleType)
                    {
                        currentPlayers.Add(user, Role.Alt_rDPS);
                        break;
                    }

                    if (ignoreRoleType)
                    {
                        if (!currentPlayers.ContainsValue(Role.mDPS1))
                        {
                            currentPlayers.Add(user, Role.mDPS1);
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS2))
                        {
                            currentPlayers.Add(user, Role.mDPS2);
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS3))
                        {
                            currentPlayers.Add(user, Role.mDPS3);
                        }
                        else if (!currentPlayers.ContainsValue(Role.mDPS4))
                        {
                            currentPlayers.Add(user, Role.mDPS4);
                        }
                        else
                        {
                            currentPlayers.Add(user, Role.Alt_mDPS);
                        }
                    }
                    break;
                case Role.T:
                    if (!currentPlayers.ContainsValue(Role.MT))
                    {
                        currentPlayers.Add(user, Role.MT);
                    }
                    else if (!currentPlayers.ContainsValue(Role.OT))
                    {
                        currentPlayers.Add(user, Role.OT);
                    }
                    else if (availableRoles.Contains(Role.OT2) && !currentPlayers.ContainsValue(Role.OT2))
                    {
                        currentPlayers.Add(user, Role.OT2);
                    }
                    else
                    {
                        currentPlayers.Add(user, Role.Alt_Tank);
                    }
                    break;
                case Role.H:
                    if (!currentPlayers.ContainsValue(Role.H1))
                    {
                        currentPlayers.Add(user, Role.H1);
                    }
                    else if (!currentPlayers.ContainsValue(Role.H2))
                    {
                        currentPlayers.Add(user, Role.H2);
                    }
                    else if (availableRoles.Contains(Role.KH) && !currentPlayers.ContainsValue(Role.KH))
                    {
                        currentPlayers.Add(user, Role.KH);
                    }
                    else if (availableRoles.Contains(Role.GH) && !currentPlayers.ContainsValue(Role.GH))
                    {
                        currentPlayers.Add(user, Role.GH);
                    }
                    else
                    {
                        currentPlayers.Add(user, Role.Alt_Healer);
                    }
                    break;
                #endregion

                #region Tanks
                case Role.MT:
                    AddPlayer(user, Role.MT);
                    break;
                case Role.OT:
                    if (!currentPlayers.ContainsValue(Role.OT))
                        AddPlayer(user, Role.OT);
                    else
                        AddPlayer(user, Role.OT2);
                    break;
                case Role.OT2:
                    AddPlayer(user, Role.OT2);
                    break;
                #endregion

                #region Healers
                case Role.H1:
                    AddPlayer(user, Role.H1);
                    break;
                case Role.H2:
                    AddPlayer(user, Role.H2);
                    break;
                case Role.GH:
                    AddPlayer(user, Role.GH);
                    break;
                case Role.KH:
                    AddPlayer(user, Role.KH);
                    break;
                #endregion

                #region rDPS
                case Role.rDPS1:
                    AddPlayer(user, Role.rDPS1);
                    break;
                case Role.rDPS2:
                    AddPlayer(user, Role.rDPS2);
                    break;
                case Role.rDPS3:
                    AddPlayer(user, Role.rDPS3);
                    break;
                case Role.rDPS4:
                    AddPlayer(user, Role.rDPS4);
                    break;
                #endregion

                #region mDPS
                case Role.mDPS1:
                    AddPlayer(user, Role.mDPS1);
                    break;
                case Role.mDPS2:
                    AddPlayer(user, Role.mDPS2);
                    break;
                case Role.mDPS3:
                    AddPlayer(user, Role.mDPS3);
                    break;
                case Role.mDPS4:
                    AddPlayer(user, Role.mDPS4);
                    break;
                    #endregion
            }
            return true;
        }

        private string GenerateRole(string role, Role r)
        {
            return $"{role}: {currentPlayers.FirstOrDefault(x => x.Value == r).Key}\n";
        }

        private string GenerateAlts()
        {
            string ret = "";
            foreach (var player in currentPlayers.Where(x => (x.Value >= Role.Alt_mDPS && x.Value <= Role.Alt_Healer)))
            {
                switch (player.Value)
                {
                    case Role.Alt_Healer:
                        ret += $"\nAlt Healer: {player.Key}";
                        break;
                    case Role.Alt_mDPS:
                        if (ignoreRoleType)
                            ret += $"\nAlt DPS: {player.Key}";
                        else
                            ret += $"\nAlt Melee: {player.Key}";
                        break;
                    case Role.Alt_rDPS:
                        if (ignoreRoleType)
                            ret += $"\nAlt DPS: {player.Key}";
                        else
                            ret += $"\nAlt Ranged: {player.Key}";
                        break;
                    case Role.Alt_Tank:
                        ret += $"\nAlt Tank: {player.Key}";
                        break;
                }
            }
            return ret;
        }

        private void AddPlayer(string user, Role role)
        {
            if (availableRoles.Contains(role) && !currentPlayers.ContainsValue(role))
            {
                currentPlayers.Add(user, role);
            }
            else
            {
                if (role >= Role.T && role <= Role.OT2)
                {
                    currentPlayers.Add(user, Role.Alt_Tank);
                }
                else if (role >= Role.H && role <= Role.KH)
                {
                    currentPlayers.Add(user, Role.Alt_Healer);
                }
                else if (role >= Role.rDPS && role <= Role.rDPS4)
                {
                    currentPlayers.Add(user, Role.Alt_rDPS);
                }
                else if (role >= Role.mDPS && role <= Role.mDPS4)
                {
                    currentPlayers.Add(user, Role.Alt_mDPS);
                }
            }
        }

        private Role GetRole(string role)
        {
            role = role.ToLower();
            Role r;
            switch (role)
            {
                #region Generics
                case "melee":
                case "mdps":
                case "dps":
                case "m":
                    r = Role.mDPS;
                    break;
                case "range":
                case "ranged":
                case "rdps":
                case "r":
                    r = Role.rDPS;
                    break;
                case "tank":
                case "t":
                    r = Role.T;
                    break;
                case "healer":
                case "h":
                    r = Role.H;
                    break;
                #endregion

                #region Healers
                case "h1":
                case "healer1":
                    r = Role.H1;
                    break;
                case "h2":
                case "healer2":
                    r = Role.H2;
                    break;
                case "kite":
                case "kitehealer":
                case "kh":
                    r = Role.KH;
                    break;
                case "group":
                case "grouphealer":
                case "gh":
                    r = Role.GH;
                    break;
                #endregion

                #region Tanks
                case "mt":
                case "maintank":
                    r = Role.MT;
                    break;
                case "ot":
                case "offtank":
                    r = Role.OT;
                    break;
                case "ot2":
                case "offtank2":
                    r = Role.OT2;
                    break;
                #endregion

                #region Ranged
                case "rdps1":
                case "ranged1":
                case "r1":
                    r = Role.rDPS1;
                    break;
                case "rdps2":
                case "ranged2":
                case "r2":
                    r = Role.rDPS2;
                    break;
                case "rdps3":
                case "ranged3":
                case "r3":
                    r = Role.rDPS3;
                    break;
                case "rdps4":
                case "ranged4":
                case "r4":
                    r = Role.rDPS4;
                    break;
                #endregion

                #region Melee
                case "mdps1":
                case "melee1":
                case "m1":
                    r = Role.mDPS1;
                    break;
                case "mdps2":
                case "melee2":
                case "m2":
                    r = Role.mDPS2;
                    break;
                case "mdps3":
                case "melee3":
                case "m3":
                    r = Role.mDPS3;
                    break;
                case "mdps4":
                case "melee4":
                case "m4":
                    r = Role.mDPS4;
                    break;
                #endregion

                default:
                    r = Role.InvalidRole;
                    break;
            }
            return r;
        }
    }
}
