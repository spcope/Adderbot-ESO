using System;
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
        T, MT, OT, OT2, H, H1, H2, GH, KH, DPS, rDPS, rDPS1, rDPS2, rDPS3, rDPS4,
        rDPS5, rDPS6, rDPS7, rDPS8, mDPS, mDPS1, mDPS2, mDPS3, mDPS4, mDPS5,
        mDPS6, mDPS7, mDPS8, Alt_DPS, Alt_mDPS, Alt_rDPS, Alt_Tank, Alt_Healer, InvalidRole
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

        [JsonProperty("availableRoles")]
        public List<Role> AvailableRoles { get; set; }

        [JsonProperty("currentPlayers")]
        public List<AdderPlayer> CurrentPlayers { get; set; }

        [JsonProperty("allowedRoles")]
        public List<ulong> AllowedRoles { get; set; }

        [JsonIgnore]
        private int mNum;
        
        [JsonIgnore]
        private int rNum;

        [JsonIgnore]
        private int fNum;

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
        public AdderRaid(string type, ulong lead, ulong messageId, string headline, List<Role> availableRoles, 
            List<AdderPlayer> currentPlayers, List<ulong> allowedRoles)
        {
            Type = type;
            Lead = lead;
            MessageId = messageId;
            Headline = headline;
            AvailableRoles = availableRoles;
            CurrentPlayers = currentPlayers;
            AllowedRoles = allowedRoles;
            mNum = AvailableRoles.Where(x => x >= Role.mDPS && x <= Role.mDPS8).Count();
            rNum = AvailableRoles.Where(x => x >= Role.rDPS && x <= Role.rDPS8).Count();
            fNum = AvailableRoles.Where(x => x == Role.DPS).Count();
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
            string timezone, ulong lead, int mNum, int rNum, int fNum, List<ulong> ars)
        {
            Type = "";
            Lead = lead;
            MessageId = 0;
            Headline = "";
            CurrentPlayers = new List<AdderPlayer>();
            AllowedRoles = ars;
            AvailableRoles = new List<Role>();

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
            bool illegalNumDps = false;

            switch (rtype.ToLower())
            {
                #region Full Trials
                case "aa":
                    Type = RaidTypes.aa;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "hrc":
                    Type = RaidTypes.hrc;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "so":
                    Type = RaidTypes.so;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "mol":
                    Type = RaidTypes.mol;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "hof":
                    Type = RaidTypes.hof;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                #endregion

                #region Mini Trials
                case "cr+0":
                    Type = RaidTypes.cr0;
                    if ((mNum + rNum + fNum) > 7)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.OT2);
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "cr+1":
                    Type = RaidTypes.cr1;
                    if ((mNum + rNum + fNum) > 7)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.OT2);
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "cr+2":
                    Type = RaidTypes.cr2;
                    if ((mNum + rNum + fNum) > 7)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.OT2);
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "cr+3":
                    Type = RaidTypes.cr3;
                    if ((mNum + rNum + fNum) > 7)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.OT2);
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "as+0":
                    Type = RaidTypes.as0;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "as+1":
                    Type = RaidTypes.as1;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                case "as+2":
                    Type = RaidTypes.as2;
                    if ((mNum + rNum + fNum) > 8)
                        illegalNumDps = true;
                    AvailableRoles.Add(Role.GH);
                    AvailableRoles.Add(Role.KH);
                    break;
                #endregion

                default:
                    throw new ArgumentException("Raid type was invalid. Please use the following:\n" + RaidTypes.RaidTypeDescriptors);
            }
            if (illegalNumDps)
                throw new ArgumentException("Could create a raid with that many dps.");
            Headline = $"Gear up for a {RClass}{Type} on {date} @{time} {timezone}!";
            AvailableRoles.Add(Role.MT);
            AvailableRoles.Add(Role.OT);
            int i;
            for (i = 0; i < mNum; i++)
            {
                AvailableRoles.Add(Role.mDPS1 + i);
            }
            for (i = 0; i < rNum; i++)
            {
                AvailableRoles.Add(Role.rDPS1 + i);
            }
            for (i = 0; i < fNum; i++)
            {
                AvailableRoles.Add(Role.DPS);
            }
        }

        public void AddPlayer(ulong uid, Role r)
        {
            switch (r)
            {
                #region Generics
                case Role.DPS:
                    if (fNum != 0)
                    {
                        if (!TryAddDPS(uid))
                        {
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_DPS));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Could not add you as a flex DPS to the raid.");
                    }
                    break;
                case Role.mDPS:
                    if (!TryAddRole(uid, true))
                    {
                        if (!TryAddDPS(uid))
                        {
                            AddAlt(uid, Role.Alt_mDPS);
                        }
                    }
                    break;
                case Role.rDPS:
                    if (!TryAddRole(uid, false))
                    {
                        if (!TryAddDPS(uid))
                        {
                            AddAlt(uid, Role.Alt_rDPS);
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
                case Role.rDPS5:
                    _addPlayer(uid, Role.rDPS5);
                    break;
                case Role.rDPS6:
                    _addPlayer(uid, Role.rDPS6);
                    break;
                case Role.rDPS7:
                    _addPlayer(uid, Role.rDPS7);
                    break;
                case Role.rDPS8:
                    _addPlayer(uid, Role.rDPS8);
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
                case Role.mDPS5:
                    _addPlayer(uid, Role.mDPS5);
                    break;
                case Role.mDPS6:
                    _addPlayer(uid, Role.mDPS6);
                    break;
                case Role.mDPS7:
                    _addPlayer(uid, Role.mDPS7);
                    break;
                case Role.mDPS8:
                    _addPlayer(uid, Role.mDPS8);
                    break;
                    #endregion
            }
        }

        private bool TryAddDPS(ulong uid)
        {
            if(CurrentPlayers.Where(x => x.Role == Role.DPS).Count() < fNum)
            {
                CurrentPlayers.Add(new AdderPlayer(uid, Role.DPS));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddAlt(ulong uid, Role role)
        {
            CurrentPlayers.Add(new AdderPlayer(uid, role));
        }

        private bool TryAddRole(ulong uid, bool isMelee)
        {
            Role role = isMelee == true ? Role.mDPS1 : Role.rDPS1;
            int ctr = isMelee == true ? mNum : rNum;
            for (int i = 0; i < ctr; i++)
            {
                if (AvailableRoles.Contains(role + i) && !_containsRole(role + i))
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, role + i));
                    return true;
                }
            }
            return false;
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
                else if (role >= Role.rDPS && role <= Role.rDPS8)
                {
                    CurrentPlayers.Add(new AdderPlayer(uid, Role.Alt_rDPS));
                }
                else if (role >= Role.mDPS && role <= Role.mDPS8)
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

        private string GenerateRole(Role role, ulong uid)
        {
            return $"{RoleRepresentations.RoleToRepresentation.GetValueOrDefault(role)}: <@{uid}>\n";
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

