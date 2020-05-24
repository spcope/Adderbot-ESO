using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Adderbot.Constants;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Adderbot.Models
{
    public enum Role
    {
        T,
        Mt,
        Ot,
        H,
        H1,
        H2,
        Ch,
        Gh,
        Th,
        Kh,
        Dps,
        RDps,
        MDps,
        Cro,
        AltDps,
        AltMDps,
        AltRDps,
        AltTank,
        AltHealer,
        AltCro,
        InvalidRole
    }

    public partial class AdderData
    {
        [JsonProperty("Guilds")] public List<AdderGuild> Guilds { get; set; }

        public AdderData()
        {
            Guilds = new List<AdderGuild>();
        }

        [JsonConstructor]
        public AdderData(List<AdderGuild> guilds)
        {
            Guilds = guilds;
        }
    }

    public class AdderGuild
    {
        [JsonProperty("GuildId")] public ulong GuildId { get; set; }
        
        [JsonProperty("EmotesAvailable")] public bool EmotesAvailable { get; set; }

        [JsonProperty("TrialLead")] public ulong Lead { get; set; }

        [JsonProperty("ActiveRaids")] public List<AdderChannel> ActiveRaids { get; set; }

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
        public AdderGuild(ulong gid, bool emotesAvailable, ulong lead, List<AdderChannel> ars)
        {
            GuildId = gid;
            EmotesAvailable = emotesAvailable;
            Lead = lead;
            ActiveRaids = ars;
        }
    }

    public class AdderChannel
    {
        [JsonProperty("ChannelId")] public ulong ChannelId { get; set; }

        [JsonProperty("Raid")] public AdderRaid Raid { get; set; }

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

    public class AdderRaid
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("lead")] public ulong Lead { get; set; }

        [JsonProperty("messageId")] public ulong MessageId { get; set; }

        [JsonProperty("headline")] public string Headline { get; set; }

        [JsonProperty("availableRoles")] public List<Role> AvailableRoles { get; set; }

        [JsonProperty("currentPlayers")] public List<AdderPlayer> CurrentPlayers { get; set; }

        [JsonProperty("allowedRoles")] public List<ulong> AllowedRoles { get; set; }

        /// <summary>
        /// Converts from JSON to JsonRaid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lead"></param>
        /// <param name="messageId"></param>
        /// <param name="headline"></param>
        /// <param name="availableRoles"></param>
        /// <param name="currentPlayers"></param>
        /// <param name="allowedRoles"></param>
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
        }

        /// <summary>
        /// Builds JsonRaid from user data
        /// </summary>
        /// <param name="raidClass"></param>
        /// <param name="raidType"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="timezone"></param>
        /// <param name="lead"></param>
        /// <param name="fNum"></param>
        /// <param name="allowedRoles"></param>
        /// <param name="mNum"></param>
        /// <param name="rNum"></param>
        public AdderRaid(string raidClass, string raidType, string date, string time,
            string timezone, ulong lead, int mNum, int rNum, int fNum, List<ulong> allowedRoles)
        {
            Lead = lead;
            MessageId = 0;
            CurrentPlayers = new List<AdderPlayer>();
            AllowedRoles = allowedRoles;
            AvailableRoles = new List<Role>();

            string rClass;
            switch (raidClass.ToLower())
            {
                case "n":
                case "norm":
                case "normal":
                    rClass = "n";
                    break;
                case "v":
                case "vet":
                case "veteran":
                    rClass = "v";
                    break;
                case "hm":
                case "hard mode":
                case "hardmode":
                    rClass = "hm";
                    break;
                default:
                    rClass = "Invalid";
                    break;
            }

            AvailableRoles.Add(Role.Mt);
            
            var expectedDpsNum = 8;

            switch (raidType.ToLower())
            {
                #region Full Trials

                case "aa":
                    Type = RaidTypes.Aa;
                    expectedDpsNum = 9;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "hrc":
                    Type = RaidTypes.Hrc;
                    expectedDpsNum = 9;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "so":
                    Type = RaidTypes.So;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "mol":
                    Type = RaidTypes.Mol;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "hof":
                    Type = RaidTypes.Hof;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case "ss":
                    Type = RaidTypes.Ss;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ch);
                    AvailableRoles.Add(Role.Gh);
                    break;

                #endregion

                #region Mini Trials

                case "cr+0":
                    Type = RaidTypes.Cr0;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case "cr+1":
                    Type = RaidTypes.Cr1;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case "cr+2":
                    Type = RaidTypes.Cr2;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case "cr+3":
                    Type = RaidTypes.Cr3;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case "as+0":
                    Type = RaidTypes.As0;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;
                case "as+1":
                    Type = RaidTypes.As1;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;
                case "as+2":
                    Type = RaidTypes.As2;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;

                #endregion


                default:
                    throw new ArgumentException("Raid type was invalid. Please use the following:\n" +
                                                RaidTypes.RaidTypeDescriptors);
            }
            
            if (fNum == -1)
                fNum = expectedDpsNum;

            if (mNum + rNum + fNum > expectedDpsNum) throw new ArgumentException("Could create a raid with that many dps.");

            Headline = $"Gear up for a {generateRaidName(rClass, Type)} on {date} @{time} {timezone}!";

            int i;
            for (i = 0; i < mNum; i++)
            {
                AvailableRoles.Add(Role.MDps);
            }

            for (i = 0; i < rNum; i++)
            {
                AvailableRoles.Add(Role.RDps);
            }

            for (i = 0; i < fNum; i++)
            {
                AvailableRoles.Add(Role.Dps);
            }
        }

        public void AddPlayer(ulong uid, Role role, Emote emote)
        {
            if (IsAlt(role))
            {
                CurrentPlayers.Add(new AdderPlayer(uid, role, emote));
                return;
            }
            
            if (IsInvalid(role))
                throw new ArgumentException("The raid you are attempting to sign up for does not allow that role.");
            
            switch (role)
            {
                case Role.H:
                    if (CheckRoleAvailable(Role.H1))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.H1, emote));
                    else if (CheckRoleAvailable(Role.H2))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.H2, emote));
                    else if (CheckRoleAvailable(Role.Ch))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Ch, emote));
                    else if (CheckRoleAvailable(Role.Kh))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Kh, emote));
                    else if (CheckRoleAvailable(Role.Gh))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Gh, emote));
                    else
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.AltHealer, emote));
                    break;
                case Role.T:
                    if (CheckRoleAvailable(Role.Mt))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Mt, emote));
                    else if (CheckRoleAvailable(Role.Ot))
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.Ot, emote));
                    else
                        CurrentPlayers.Add(new AdderPlayer(uid, Role.AltTank, emote));
                    break;
                default:
                    if (CheckRoleAvailable(role))
                    {
                        CurrentPlayers.Add(new AdderPlayer(uid, role, emote));
                    }
                    else
                    {
                        if (IsHealer(role))
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.AltHealer, emote));
                        else if (IsTank(role))
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.AltTank, emote));
                        else if (IsRDps(role))
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.AltRDps, emote));
                        else if (IsMDps(role))
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.AltMDps, emote));
                        else
                            CurrentPlayers.Add(new AdderPlayer(uid, Role.AltDps, emote));
                    }
                    break;
            }
        }
        private String generateRaidName(String rClass, String type)
        {
            return rClass.Equals("hm") ? "v" + type + " " + rClass : rClass + type;
        }

        private bool CheckRoleAvailable(Role role)
        {
            var count = AvailableRoles.Count(x => x == role);

            if (count <= 0) throw new ArgumentException("This raid does not allow the requested role");

            return count > CurrentPlayers.Count(x => x.Role == role);
        }

        private static bool IsHealer(Role role) => Role.H <= role && role <= Role.Kh;

        private static bool IsTank(Role role) => Role.T <= role && role <= Role.Ot;

        private static bool IsRDps(Role role) => Role.RDps == role;

        private static bool IsMDps(Role role) => Role.MDps == role;

        private static bool IsAlt(Role role) => Role.AltDps <= role && role <= Role.AltHealer;

        private static bool IsInvalid(Role role) => Role.InvalidRole == role;

        private string Build()
        {
            return $"{BuildAllowedRoles()}\n" +
                   $"{Headline}\n" +
                   $"{BuildPlayers()}";
        }

        public string BuildAllowedRoles()
        {
            var result = "";
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
            var eb = new EmbedBuilder();
            switch (Type.ToLower())
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
                case "ss":
                    eb.Color = Color.Teal;
                    break;
                default:
                    throw new ArgumentException("Invalid raid type");
            }

            eb.Title = Headline;
            eb.Description = Build();
            return eb.Build();
        }

        public string BuildPlayers()
        {
            var players = "";
            var added = new List<AdderPlayer>();
            foreach (var role in AvailableRoles)
            {
                var player = CurrentPlayers.FirstOrDefault(x => x.Role == role && !added.Contains(x));
                if (player == null)
                    players += $"{RoleRepresentations.RoleToRepresentation.GetValueOrDefault(role)}:\n";
                else
                {
                    players += GenerateRole(player);
                    added.Add(player);
                }
            }

            players += "\n";

            return CurrentPlayers.Where(x => IsAlt(x.Role)).Aggregate(players,
                (current, alt) => current + $"{GenerateRole(alt)}");
        }

        private static string GenerateRole(AdderPlayer player)
        {
            return $"{RoleRepresentations.RoleToRepresentation.GetValueOrDefault(player.Role)}: <@{player.PlayerId}> {player.Emote}\n";
        }
    }

    public class AdderPlayer
    {
        [JsonProperty("player")] public ulong PlayerId { get; set; }

        [JsonProperty("emote")] public string Emote { get; set; }

        [JsonProperty("role")] public int RoleId { get; set; }

        [JsonIgnore] public readonly Role Role;

        [JsonIgnore] public readonly Emote EmoteObj;

        [JsonConstructor]
        public AdderPlayer(ulong pl, string emote, int role)
        {
            PlayerId = pl;
            Emote = emote;
            EmoteObj = emote != null ? Discord.Emote.Parse(emote) : null;
            RoleId = role;
            Role = (Role) role;
        }

        public AdderPlayer(ulong pl, Role r, Emote emote)
        {
            PlayerId = pl;
            RoleId = r.GetHashCode();
            Role = r;
            EmoteObj = emote;
            Emote = emote?.ToString();
        }
    }

    public partial class AdderData
    {
        public static AdderData FromJson(string json) =>
            JsonConvert.DeserializeObject<AdderData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AdderData self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            },
        };
    }
}