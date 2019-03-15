using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Adderbot.Models
{
    public enum Role
    {
        T, MT, OT, OT2, H, H1, H2, GH, KH, rDPS, rDPS1, rDPS2, rDPS3, rDPS4,
        mDPS, mDPS1, mDPS2, mDPS3, mDPS4, Alt_mDPS, Alt_rDPS, Alt_Tank,
        Alt_Healer, InvalidRole
    }

    public partial class AdderData
    {
        [JsonProperty("Guilds")]
        public List<AdderGuild> Guilds { get; set; }

        public AdderData()
        {
            Guilds = new List<AdderGuild>();
        }

        [JsonConstructor]
        public AdderData(List<AdderGuild> gs)
        {
            Guilds = gs;
        }
    }

    public partial class AdderGuild
    {
        [JsonProperty("GuildId")]
        public ulong GuildId { get; set; }

        [JsonProperty("TrialLead")]
        public ulong Lead { get; set; }

        [JsonProperty("ActiveRaids")]
        public List<AdderChannel> ActiveRaids { get; set; }

        public AdderGuild(ulong gid)
        {
            GuildId = gid;
            Lead = 0;
            ActiveRaids = new List<AdderChannel>();
        }

        public AdderGuild(ulong gid, ulong lead)
        {
            GuildId = gid;
            Lead = lead;
            ActiveRaids = new List<AdderChannel>();
        }

        [JsonConstructor]
        public AdderGuild(ulong gid, ulong lead, List<AdderChannel> ars)
        {
            GuildId = gid;
            Lead = lead;
            ActiveRaids = ars;
        }
    }

    public partial class AdderChannel
    {
        [JsonProperty("ChannelId")]
        public ulong ChannelId { get; set; }

        [JsonProperty("Raid")]
        public AdderRaid Raid { get; set; }

        public AdderChannel(ulong cid)
        {
            ChannelId = cid;
            Raid = null;
        }

        [JsonConstructor]
        public AdderChannel(ulong cid, AdderRaid jr)
        {
            ChannelId = cid;
            Raid = jr;
        }
    }

    public partial class AdderRaid
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("lead")]
        public ulong Lead { get; set; }

        [JsonProperty("messageId")]
        public ulong MessageId { get; set; }

        [JsonProperty("headline")]
        public string Headline { get; set; }

        [JsonProperty("ignoreRoleType")]
        public bool IgnoreRoleType { get; set; }

        [JsonProperty("noMelee")]
        public bool NoMelee { get; set; }

        [JsonProperty("availableRoles")]
        public List<Role> AvailableRoles { get; set; }

        [JsonProperty("currentPlayers")]
        public List<AdderPlayer> CurrentPlayers { get; set; }

        [JsonProperty("allowedRoles")]
        public List<ulong> AllowedRoles { get; set; }

        /// <summary>
        /// Converts from JSON to JsonRaid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lead"></param>
        /// <param name="messageId"></param>
        /// <param name="headline"></param>
        /// <param name="ignoreRoleType"></param>
        /// <param name="noMelee"></param>
        /// <param name="ars"></param>
        /// <param name="cps"></param>
        /// <param name="allowedRoleIds"></param>
        [JsonConstructor]
        public AdderRaid(string type, ulong lead, ulong messageId, string headline, bool ignoreRoleType,
            bool noMelee, List<Role> availableRoles, List<AdderPlayer> currentPlayers,
            List<ulong> allowedRoles)
        {
            Type = type;
            Lead = lead;
            MessageId = messageId;
            Headline = headline;
            IgnoreRoleType = ignoreRoleType;
            NoMelee = noMelee;
            AvailableRoles = availableRoles;
            AvailableRoles = new List<Role>();
            foreach (var id in availableRoles)
                AvailableRoles.Add((Role)id);
            CurrentPlayers = currentPlayers;
            AllowedRoles = allowedRoles;
        }

        /// <summary>
        /// Builds JsonRaid from user data
        /// </summary>
        /// <param name="rclass"></param>
        /// <param name="rtype"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="timezone"></param>
        /// <param name="lead"></param>
        /// <param name="headline"></param>
        /// <param name="ignoreRoleType"></param>
        /// <param name="noMelee"></param>
        /// <param name="ars"></param>
        public AdderRaid(string rclass, string rtype, string date, string time,
            string timezone, ulong lead, bool noMelee, List<ulong> ars)
        {
            Type = "";
            Lead = lead;
            MessageId = 0;
            Headline = "";
            IgnoreRoleType = false;
            NoMelee = noMelee;
            CurrentPlayers = new List<AdderPlayer>();
            AllowedRoles = ars;

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

            switch (rtype.ToLower())
            {
                #region Full Trials
                case "aa":
                    Headline = $"Gear up for a {RClass}AA on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>() {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    IgnoreRoleType = true;
                    Type = "aa";
                    break;
                case "hrc":
                    Headline = $"Gear up for a {RClass}HRC on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>() {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "hrc";
                    break;
                case "so":
                    Headline = $"Gear up for a {RClass}SO on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    IgnoreRoleType = true;
                    Type = "so";
                    break;
                case "mol":
                    Headline = $"Gear up for a {RClass}MoL on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "mol";
                    break;
                case "hof":
                    Headline = $"Gear up for a {RClass}HoF on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "hof";
                    break;
                #endregion

                #region Mini Trials
                case "cr+0":
                    Headline = $"Gear up for a {RClass}CR+0 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.H1, Role.H2, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    Type = "cr+0";
                    NoMelee = noMelee;
                    break;
                case "cr+1":
                    Headline = $"Gear up for a {RClass}CR+1 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    Type = "cr+1";
                    NoMelee = noMelee;
                    break;
                case "cr+2":
                    Headline = $"Gear up for a {RClass}CR+2 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    Type = "cr+2";
                    NoMelee = noMelee;
                    break;
                case "cr+3":
                    Headline = $"Gear up for a {RClass}CR+3 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.OT2, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3
                    };
                    Type = "cr+3";
                    NoMelee = noMelee;
                    break;
                case "as+0":
                    Headline = $"Gear up for a {RClass}AS+0 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "as+0";
                    NoMelee = noMelee;
                    break;
                case "as+1":
                    Headline = $"Gear up for a {RClass}AS+1 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "as+1";
                    NoMelee = noMelee;
                    break;
                case "as+2":
                    Headline = $"Gear up for a {RClass}AS+2 on {date} @{time} {timezone}!";
                    AvailableRoles = new List<Role>()
                    {
                        Role.MT, Role.OT, Role.KH, Role.GH, Role.mDPS1,
                        Role.mDPS2, Role.mDPS3, Role.mDPS4,  Role.rDPS1,
                        Role.rDPS2, Role.rDPS3, Role.rDPS4
                    };
                    Type = "as+2";
                    NoMelee = noMelee;
                    break;
                    #endregion
            }
        }

        public void AddPlayer(ulong uid, Role r)
        {
            switch (r)
            {
                #region Generics
                case Role.mDPS:
                    if (!NoMelee)
                    {
                        if (!_containsRole(Role.mDPS1))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS1));
                            break;
                        }
                        else if (!_containsRole(Role.mDPS2))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS2));
                            break;
                        }
                        else if (!_containsRole(Role.mDPS3))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS3));
                            break;
                        }
                        else if (!_containsRole(Role.mDPS4))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS4));
                            break;
                        }
                        else if (!IgnoreRoleType)
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_mDPS));
                            break;
                        }
                        else
                        {
                            if (!_containsRole(Role.rDPS1))
                            {
                                CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS1));
                            }
                            else if (!_containsRole(Role.rDPS2))
                            {
                                CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS2));
                            }
                            else if (!_containsRole(Role.rDPS3))
                            {
                                CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS3));
                            }
                            else if (!_containsRole(Role.rDPS4))
                            {
                                CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS4));
                            }
                            else
                            {
                                CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_rDPS));
                            }
                        }
                    }
                    break;
                case Role.rDPS:
                    if (!_containsRole(Role.rDPS1))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS1));
                        break;
                    }
                    else if (!_containsRole(Role.rDPS2))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS2));
                        break;
                    }
                    else if (!_containsRole(Role.rDPS3))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS3));
                        break;
                    }
                    else if (!_containsRole(Role.rDPS4))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.rDPS4));
                        break;
                    }
                    else if (!IgnoreRoleType)
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_rDPS));
                        break;
                    }
                    else
                    {
                        if (!_containsRole(Role.mDPS1))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS1));
                        }
                        else if (!_containsRole(Role.mDPS2))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS2));
                        }
                        else if (!_containsRole(Role.mDPS3))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS3));
                        }
                        else if (!_containsRole(Role.mDPS4))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.mDPS4));
                        }
                        else
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_mDPS));
                        }
                    }
                    break;
                case Role.T:
                    if (!_containsRole(Role.MT))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.MT));
                    }
                    else if (!_containsRole(Role.OT))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.OT));
                    }
                    else if (AvailableRoles.Contains(Role.OT2) && !_containsRole(Role.OT2))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.OT2));
                    }
                    else
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_Tank));
                    }
                    break;
                case Role.H:
                    if (!_containsRole(Role.H1))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.H1));
                    }
                    else if (!_containsRole(Role.H2))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.H2));
                    }
                    else if (AvailableRoles.Contains(Role.KH) && !_containsRole(Role.KH))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.KH));
                    }
                    else if (AvailableRoles.Contains(Role.GH) && !_containsRole(Role.GH))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.GH));
                    }
                    else
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_Healer));
                    }
                    break;
                #endregion

                #region Tanks
                case Role.MT:
                    _addPlayer(uid, Role.MT);
                    break;
                case Role.OT:
                    if (!_containsRole(Role.OT))
                        _addPlayer(uid, Role.OT);
                    else
                        _addPlayer(uid, Role.OT2);
                    break;
                case Role.OT2:
                    _addPlayer(uid, Role.OT2);
                    break;
                #endregion

                #region Healers
                case Role.H1:
                    _addPlayer(uid, Role.H1);
                    break;
                case Role.H2:
                    _addPlayer(uid, Role.H2);
                    break;
                case Role.GH:
                    _addPlayer(uid, Role.GH);
                    break;
                case Role.KH:
                    _addPlayer(uid, Role.KH);
                    break;
                #endregion

                #region rDPS
                case Role.rDPS1:
                    _addPlayer(uid, Role.rDPS1);
                    break;
                case Role.rDPS2:
                    _addPlayer(uid, Role.rDPS2);
                    break;
                case Role.rDPS3:
                    _addPlayer(uid, Role.rDPS3);
                    break;
                case Role.rDPS4:
                    _addPlayer(uid, Role.rDPS4);
                    break;
                #endregion

                #region mDPS
                case Role.mDPS1:
                    _addPlayer(uid, Role.mDPS1);
                    break;
                case Role.mDPS2:
                    _addPlayer(uid, Role.mDPS2);
                    break;
                case Role.mDPS3:
                    _addPlayer(uid, Role.mDPS3);
                    break;
                case Role.mDPS4:
                    _addPlayer(uid, Role.mDPS4);
                    break;
                    #endregion
            }
        }

        private void _addPlayer(ulong uid, Role role)
        {
            if (AvailableRoles.Contains(role) && !_containsRole(role))
            {
                CurrentPlayers.Add(new AdderPlayer(uid, role));
            }
            else
            {
                if (role >= Role.T && role <= Role.OT2)
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_Tank));
                }
                else if (role >= Role.H && role <= Role.KH)
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_Healer));
                }
                else if (role >= Role.rDPS && role <= Role.rDPS4)
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_rDPS));
                }
                else if (role >= Role.mDPS && role <= Role.mDPS4)
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_mDPS));
                }
            }
        }

        private bool _containsRole(Role r)
        {
            foreach (var cp in CurrentPlayers)
            {
                if (cp.Role == r)
                    return true;
            }
            return false;
        }

        public string Build()
        {
            string result = "";
            switch (Type)
            {
                #region Full 
                case "so":
                case "aa":
                    result += _generateRole(RoleRepresentations.MT, Role.MT) + _generateRole(RoleRepresentations.OT, Role.OT)
                        + _generateRole(RoleRepresentations.H1, Role.H1) + _generateRole(RoleRepresentations.H2, Role.H2)
                        + _generateRole(RoleRepresentations.DPS, Role.mDPS1) + _generateRole(RoleRepresentations.DPS, Role.mDPS2)
                        + _generateRole(RoleRepresentations.DPS, Role.mDPS3) + _generateRole(RoleRepresentations.DPS, Role.mDPS4)
                        + _generateRole(RoleRepresentations.DPS, Role.rDPS1) + _generateRole(RoleRepresentations.DPS, Role.rDPS2)
                        + _generateRole(RoleRepresentations.DPS, Role.rDPS3) + _generateRole(RoleRepresentations.DPS, Role.rDPS4)
                        + _generateAlts();
                    break;
                case "hof":
                case "mol":
                case "hrc":
                    result += _generateRole(RoleRepresentations.MT, Role.MT) + _generateRole(RoleRepresentations.OT, Role.OT)
                        + _generateRole(RoleRepresentations.H1, Role.H1) + _generateRole(RoleRepresentations.H2, Role.H2)
                        + _generateRole($"{RoleRepresentations.mDPS} 1", Role.mDPS1) + _generateRole($"{RoleRepresentations.mDPS} 2", Role.mDPS2)
                        + _generateRole($"{RoleRepresentations.mDPS} 3", Role.mDPS3) + _generateRole($"{RoleRepresentations.mDPS} 4", Role.mDPS4)
                        + _generateRole($"{RoleRepresentations.rDPS} 1", Role.rDPS1) + _generateRole($"{RoleRepresentations.rDPS} 2", Role.rDPS2)
                        + _generateRole($"{RoleRepresentations.rDPS} 3", Role.rDPS3) + _generateRole($"{RoleRepresentations.rDPS} 4", Role.rDPS4)
                        + _generateAlts();
                    break;
                #endregion

                #region Mini Trials
                case "cr+0":
                case "cr+1":
                case "cr+2":
                case "cr+3":
                    result += _generateRole(RoleRepresentations.MT, Role.MT) + _generateRole(RoleRepresentations.OT, Role.OT)
                        + _generateRole(RoleRepresentations.OT2, Role.OT2) + _generateRole(RoleRepresentations.H1, Role.H1)
                        + _generateRole(RoleRepresentations.H2, Role.H2);
                    if (NoMelee)
                    {
                        result += _generateRole($"{RoleRepresentations.rDPS} 1", Role.mDPS1) + _generateRole($"{RoleRepresentations.rDPS} 2", Role.mDPS2)
                            + _generateRole($"{RoleRepresentations.rDPS} 3", Role.mDPS3) + _generateRole($"{RoleRepresentations.rDPS} 4", Role.mDPS4)
                            + _generateRole($"{RoleRepresentations.rDPS} 5", Role.rDPS1) + _generateRole($"{RoleRepresentations.rDPS} 6", Role.rDPS2)
                            + _generateRole($"{RoleRepresentations.rDPS} 7", Role.rDPS3);
                    }
                    else
                    {
                        result += _generateRole($"{RoleRepresentations.DPS} 1", Role.mDPS1) + _generateRole($"{RoleRepresentations.DPS} 2", Role.mDPS2)
                           + _generateRole($"{RoleRepresentations.DPS} 3", Role.mDPS3) + _generateRole($"{RoleRepresentations.DPS} 4", Role.mDPS4)
                           + _generateRole($"{RoleRepresentations.DPS} 5", Role.rDPS1) + _generateRole($"{RoleRepresentations.DPS} 6", Role.rDPS2)
                           + _generateRole($"{RoleRepresentations.DPS} 7", Role.rDPS3);
                    }
                    result += _generateAlts();
                    break;
                case "as+0":
                case "as+1":
                case "as+2":
                    result += _generateRole(RoleRepresentations.MT, Role.MT) + _generateRole(RoleRepresentations.OT, Role.OT)
                        + _generateRole(RoleRepresentations.H1, Role.H1) + _generateRole(RoleRepresentations.H2, Role.H2);
                    if (NoMelee)
                    {
                        result += _generateRole($"{RoleRepresentations.rDPS} 1", Role.mDPS1) + _generateRole($"{RoleRepresentations.rDPS} 2", Role.mDPS2)
                            + _generateRole($"{RoleRepresentations.rDPS} 3", Role.mDPS3) + _generateRole($"{RoleRepresentations.rDPS} 4", Role.mDPS4)
                            + _generateRole($"{RoleRepresentations.rDPS} 5", Role.rDPS1) + _generateRole($"{RoleRepresentations.rDPS} 6", Role.rDPS2)
                            + _generateRole($"{RoleRepresentations.rDPS} 7", Role.rDPS3) + _generateRole($"{RoleRepresentations.rDPS} 8", Role.rDPS4);
                    }
                    else
                    {
                        result += _generateRole($"{RoleRepresentations.DPS} 1", Role.mDPS1) + _generateRole($"{RoleRepresentations.DPS} 2", Role.mDPS2)
                           + _generateRole($"{RoleRepresentations.DPS} 3", Role.mDPS3) + _generateRole($"{RoleRepresentations.DPS} 4", Role.mDPS4)
                           + _generateRole($"{RoleRepresentations.DPS} 5", Role.rDPS1) + _generateRole($"{RoleRepresentations.DPS} 6", Role.rDPS2)
                           + _generateRole($"{RoleRepresentations.DPS} 7", Role.rDPS3) + _generateRole($"{RoleRepresentations.DPS} 8", Role.rDPS4);
                    }
                    result += _generateAlts();
                    break;
                    #endregion
            }
            return result;
        }

        public string BuildAllowedRoles()
        {
            string result = "";
            foreach (var role in AllowedRoles)
            {
                if (role == 0)
                    result += "@everyone ";
                else
                    result += $"<@&{role}> ";
            }
            return result;
        }

        public Embed BuildEmbed()
        {
            EmbedBuilder eb = new EmbedBuilder();
            switch (Type)
            {
                case "aa":
                    eb.Color = Color.Magenta;
                    break;
                case "so":
                    eb.Color = Color.Green;
                    break;
                case "hrc":
                    eb.Color = Color.LightGrey;
                    break;
                case "mol":
                    eb.Color = Color.DarkBlue;
                    break;
                case "hof":
                    eb.Color = Color.Gold;
                    break;
                case "as+0":
                case "as+1":
                case "as+2":
                    eb.Color = Color.DarkRed;
                    break;
                case "cr+0":
                case "cr+1":
                case "cr+2":
                case "cr+3":
                    eb.Color = Color.DarkPurple;
                    break;
            }
            eb.Title = Headline;
            eb.Description = Build();
            return eb.Build();
        }

        private string _generateRole(string role, Role r)
        {
            var pl = CurrentPlayers.FirstOrDefault(x => x.Role == r);
            string user = "";
            if (pl != null)
            {
                user = $"<@{CurrentPlayers.Find(x => x.Role == r).PlayerId}>";
            }
            return $"{role}: {user}\n";
        }

        private string _generateAlts()
        {
            string ret = "";
            foreach (var player in CurrentPlayers.FindAll(x => (x.Role >= Role.Alt_mDPS && x.Role <= Role.Alt_Healer)))
            {
                switch (player.Role)
                {
                    case Role.Alt_Healer:
                        ret += $"\n{RoleRepresentations.Alt_Healer}: <@{player.PlayerId}>";
                        break;
                    case Role.Alt_mDPS:
                        if (IgnoreRoleType)
                            ret += $"\n{RoleRepresentations.Alt_DPS}: <@{player.PlayerId}>";
                        else
                            ret += $"\n{RoleRepresentations.Alt_mDPS}: <@{player.PlayerId}>";
                        break;
                    case Role.Alt_rDPS:
                        if (IgnoreRoleType)
                            ret += $"\n{RoleRepresentations.Alt_DPS}: <@{player.PlayerId}>";
                        else
                            ret += $"\n{RoleRepresentations.Alt_rDPS}: <@{player.PlayerId}>";
                        break;
                    case Role.Alt_Tank:
                        ret += $"\n{RoleRepresentations.Alt_Tank}: <@{player.PlayerId}>";
                        break;
                }
            }
            return ret;
        }
    }

    public partial class AdderPlayer
    {
        [JsonProperty("player")]
        public ulong PlayerId { get; set; }

        [JsonProperty("role")]
        public int RoleId { get; set; }

        [JsonIgnore]
        public Role Role;

        [JsonConstructor]
        public AdderPlayer(ulong pl, int role)
        {
            PlayerId = pl;
            RoleId = role;
            Role = (Role)role;
        }

        public AdderPlayer(ulong pl, Role r)
        {
            PlayerId = pl;
            RoleId = r.GetHashCode();
            Role = r;
        }
    }

    public partial class AdderData
    {
        public static AdderData FromJson(string json) => JsonConvert.DeserializeObject<AdderData>(json, Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AdderData self) => JsonConvert.SerializeObject(self, Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

