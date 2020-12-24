using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Adderbot.Constants;
using Adderbot.Helpers;
using Discord;
using Newtonsoft.Json;

namespace Adderbot.Models
{
    public class AdderRaid
    {
        /// <summary>
        /// The class of the raid (n, v, hm)
        /// </summary>
        [JsonProperty("raidclass")]
        public RaidClass RaidClass { get; set; }

        /// <summary>
        /// The type of the raid (n, v, hm)
        /// </summary>
        [JsonProperty("type")]
        public RaidType Type { get; set; }

        /// <summary>
        /// The date of the raid
        /// </summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>
        /// The time of the raid
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }
        
        /// <summary>
        /// The DateTime of the raid
        /// </summary>
        [JsonProperty("datetime")]
        public DateTime DateTimeObj { get; set; }

        /// <summary>
        /// The timezone of the raid
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// The ID of the Discord user who is leading the trial
        /// </summary>
        [JsonProperty("lead")]
        public ulong Lead { get; set; }

        /// <summary>
        /// The ID of the actual message the raid is housed in
        /// </summary>
        [JsonProperty("messageId")]
        public ulong MessageId { get; set; }

        /// <summary>
        /// List of the roles in the raid (MT, OT, rDPS, etc)
        /// </summary>
        [JsonProperty("availableRoles")]
        public SynchronizedCollection<Role> AvailableRoles { get; set; }

        /// <summary>
        /// List of current players signed up for the raid
        /// </summary>
        [JsonProperty("currentPlayers")]
        public SynchronizedCollection<AdderPlayer> CurrentPlayers { get; set; }

        /// <summary>
        /// List of discord roles that can sign up for the raid
        /// </summary>
        [JsonProperty("allowedRoles")]
        public SynchronizedCollection<ulong> AllowedRoles { get; set; }

        /// <summary>
        /// Override for the headline of a raid
        /// </summary>
        [JsonProperty("headlineOverride")]
        public bool HeadlineOverride { get; set; }

        /// <summary>
        /// Overriden headline. Will not be used for the raid headline unless HeadlineOverride = true
        /// </summary>
        [JsonProperty("overridenHeadline")]
        public string OverridenHeadline { get; set; }

        /// <summary>
        /// Converts from JSON to AdderRaid
        /// </summary>
        /// <param name="raidClass"></param>
        /// <param name="type"></param>
        /// <param name="lead"></param>
        /// <param name="messageId"></param>
        /// <param name="date"></param>
        /// <param name="timezone"></param>
        /// <param name="availableRoles"></param>
        /// <param name="currentPlayers"></param>
        /// <param name="allowedRoles"></param>
        /// <param name="time"></param>
        /// <param name="headlineOverride"></param>
        /// <param name="overridenHeadline"></param>
        [JsonConstructor]
        public AdderRaid(RaidClass raidClass, RaidType type, string date, string time, string timezone, ulong lead,
            ulong messageId, SynchronizedCollection<Role> availableRoles,
            SynchronizedCollection<AdderPlayer> currentPlayers, SynchronizedCollection<ulong> allowedRoles,
            bool headlineOverride, string overridenHeadline)
        {
            RaidClass = raidClass;
            Type = type;
            Lead = lead;
            MessageId = messageId;
            Date = date;
            Time = time;
            Timezone = timezone;
            AvailableRoles = availableRoles;
            CurrentPlayers = currentPlayers;
            AllowedRoles = allowedRoles;
            HeadlineOverride = headlineOverride;
            OverridenHeadline = overridenHeadline;
        }

        /// <summary>
        /// Builds AdderRaid from user data
        /// </summary>
        /// <param name="raidClass"></param>
        /// <param name="raidType"></param>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="timezone"></param>
        /// <param name="lead"></param>
        /// <param name="fNum"></param>
        /// <param name="nNum"></param>
        /// <param name="allowedRoles"></param>
        /// <param name="mNum"></param>
        /// <param name="rNum"></param>
        public AdderRaid(string raidClass, string raidType, string date, string time,
            string timezone, ulong lead, int mNum, int rNum, int fNum, int nNum,
            SynchronizedCollection<ulong> allowedRoles)
        {
            var dt = TimeHelper.ParseTime(date, time, timezone);
            if (dt == DateTime.UnixEpoch) throw new ArgumentException(MessageText.Error.InvalidTime);

            DateTimeObj = dt;
            Lead = lead;
            MessageId = 0;
            CurrentPlayers = new SynchronizedCollection<AdderPlayer>();
            AllowedRoles = allowedRoles;
            AvailableRoles = new SynchronizedCollection<Role>();
            Date = date;
            Time = time;
            Timezone = timezone;
            HeadlineOverride = false;
            OverridenHeadline = string.Empty;

            // Determine the class of raid (normal, veteran, hard mode)
            switch (raidClass.ToLower())
            {
                case Keywords.Create.N:
                case Keywords.Create.Norm:
                case Keywords.Create.Normal:
                    RaidClass = RaidClass.Norm;
                    break;
                case Keywords.Create.V:
                case Keywords.Create.Vet:
                case Keywords.Create.Veteran:
                    RaidClass = RaidClass.Vet;
                    break;
                case Keywords.Create.HM:
                case Keywords.Create.Hardmode:
                    RaidClass = RaidClass.Hardmode;
                    break;
                default:
                    RaidClass = RaidClass.Invalid;
                    break;
            }

            // All trials have a main tank, add it to the list of roles
            AvailableRoles.Add(Role.Mt);

            // In most cases there are 8 dps in a raid
            var expectedDpsNum = 8;

            // Switch over raid types and set allowed roles and number of dps accordingly
            switch (raidType.ToLower())
            {
                #region Full Trials

                case Keywords.Create.Aa:
                    Type = RaidType.Aa;
                    expectedDpsNum = 9;
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.Hrc:
                    Type = RaidType.Hrc;
                    if (RaidClass == RaidClass.Hardmode)
                    {
                        expectedDpsNum = 8;
                        AvailableRoles.Add(Role.Ot);
                    }
                    else
                    {
                        expectedDpsNum = 9;
                    }

                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.So:
                    Type = RaidType.So;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.Mol:
                    Type = RaidType.Mol;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.Hof:
                    Type = RaidType.Hof;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.Ss:
                    Type = RaidType.Ss;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ch);
                    AvailableRoles.Add(Role.Gh);
                    break;
                case Keywords.Create.SsI:
                    Type = RaidType.SsI;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ch);
                    AvailableRoles.Add(Role.Gh);
                    break;
                case Keywords.Create.SsF:
                    Type = RaidType.SsF;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ch);
                    AvailableRoles.Add(Role.Gh);
                    break;
                case Keywords.Create.SsFI:
                    Type = RaidType.SsFI;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ch);
                    AvailableRoles.Add(Role.Gh);
                    break;
                case Keywords.Create.Ka:
                    Type = RaidType.Ka;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.KaV:
                    Type = RaidType.KaV;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.KaY:
                    Type = RaidType.KaY;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;
                case Keywords.Create.KaVY:
                    Type = RaidType.KaVY;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.H1);
                    AvailableRoles.Add(Role.H2);
                    break;

                #endregion

                #region Mini Trials

                case Keywords.Create.Cr0:
                    Type = RaidType.Cr0;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case Keywords.Create.Cr1:
                    Type = RaidType.Cr1;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case Keywords.Create.Cr2:
                    Type = RaidType.Cr2;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case Keywords.Create.Cr3:
                    Type = RaidType.Cr3;
                    expectedDpsNum = 7;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Kh);
                    break;
                case Keywords.Create.As0:
                    Type = RaidType.As0;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;
                case Keywords.Create.As1:
                    Type = RaidType.As1;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;
                case Keywords.Create.As2:
                    Type = RaidType.As2;
                    AvailableRoles.Add(Role.Ot);
                    AvailableRoles.Add(Role.Gh);
                    AvailableRoles.Add(Role.Th);
                    break;

                #endregion

                default:
                    throw new ArgumentException("Raid type was invalid. Please use the following:\n" +
                                                CommandHelp.RaidLeadHelp.RaidTypeDescriptors);
            }

            // If fNum is -1, make all dps flex
            if (fNum == -1)
                fNum = expectedDpsNum;

            // Check that the number of dps is the required number
            if ((mNum + rNum + fNum + nNum) != expectedDpsNum)
                throw new ArgumentException("Could create a raid with that many dps.");

            int i;

            // Add the number of dps to the raid based on the input numbers of melee, ranged, flex, and necro
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

            for (i = 0; i < nNum; i++)
            {
                AvailableRoles.Add(Role.Cro);
            }
        }

        /// <summary>
        /// Adds a player to a raid
        /// </summary>
        /// <param name="uid">The Discord ID of the player</param>
        /// <param name="role">The role the player should be added as</param>
        /// <param name="emote">The emote (stamcro, magplar, etc) associated with the input</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddPlayer(ulong uid, Role role, Emote emote)
        {
            // If the role is an alt role, just add to the raid
            if (IsAlt(role))
            {
                CurrentPlayers.Add(new AdderPlayer(uid, role, emote));
                return;
            }

            // If the role is invalid or does not fit in the raid (i.e. H1 in a vSS), throw and return
            if (IsInvalid(role))
                throw new ArgumentException("The raid you are attempting to sign up for does not allow that role.");

            // Add player with role if available
            switch (role)
            {
                // Handle generic healer role
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
                // Handle generic tank role
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

        /// <summary>
        /// Generates a string of the raid name
        /// </summary>
        /// <param name="rClass">The class of the raid (norm, vet, hm)</param>
        /// <param name="type">The type of the raid (aa, hrc, etc)</param>
        /// <returns></returns>
        private static string GenerateRaidName(RaidClass rClass, RaidType type)
        {
            switch (rClass)
            {
                case RaidClass.Norm:
                    return "n" + RaidTypeRepresentation.GetRepresentation(type);
                case RaidClass.Vet:
                    return "v" + RaidTypeRepresentation.GetRepresentation(type);
                case RaidClass.Hardmode:
                    return "v" + RaidTypeRepresentation.GetRepresentation(type) + " " + "HM";
                default:
                    return "Invalid raid";
            }
        }

        /// <summary>
        /// Checks if the role is available in the raid
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private bool CheckRoleAvailable(Role role)
        {
            var count = AvailableRoles.Count(x => x == role);

            if (count <= 0) throw new ArgumentException("This raid does not allow the requested role");

            return count > CurrentPlayers.Count(x => x.Role == role);
        }

        /// <summary>
        /// Returns if the role is a healer role or not
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsHealer(Role role) => Role.H <= role && role <= Role.Kh;

        /// <summary>
        /// Returns if the role is a tank role or not
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsTank(Role role) => Role.T <= role && role <= Role.Ot;

        /// <summary>
        /// Returns if the role is a ranged dps
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsRDps(Role role) => Role.RDps == role;

        /// <summary>
        /// Returns if the role is a melee dps
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsMDps(Role role) => Role.MDps == role;

        /// <summary>
        /// Returns if the role is an alt
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsAlt(Role role) => Role.AltDps <= role && role <= Role.AltCro;

        /// <summary>
        /// Returns if the role is invalid
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool IsInvalid(Role role) => Role.InvalidRole == role;

        /// <summary>
        /// Builds the full raid text
        /// </summary>
        /// <returns></returns>
        private string Build()
        {
            return $"**Trial:** {GenerateRaidName(RaidClass, Type)}\n" +
                   $"{BuildDate()}\n" +
                   $"{BuildTime()}\n" +
                   $"{BuildLead()}\n" +
                   $"**-----------------------------------------------**\n" +
                   $"{BuildPlayers()}";
        }

        /// <summary>
        /// Generates the raid's date
        /// </summary>
        /// <returns></returns>
        private string BuildDate()
        {
            return DateTimeFormatInfo.CurrentInfo != null ? $"**Date:** {DateTimeObj:dddd, MMMM dd yyyy}" : null;
        }

        private string BuildTime()
        {
            return $"**Time:** {Time} {Timezone}";
        }

        /// <summary>
        /// Generates the raid's headline
        /// </summary>
        /// <returns></returns>
        private string GenerateHeadline()
        {
            return HeadlineOverride ? OverridenHeadline : $"Gear up for a {GenerateRaidName(RaidClass, Type)} on {Date} @{Time} {Timezone}!";
        }

        /// <summary>
        /// Builds the trial lead text
        /// </summary>
        /// <returns></returns>
        private string BuildLead()
        {
            return $"**Leader:** {MentionUtils.MentionUser(Lead)}";
        }

        /// <summary>
        /// Builds the allowed roles text
        /// </summary>
        /// <returns></returns>
        public string BuildAllowedRoles()
        {
            var result = "";
            foreach (var role in AllowedRoles)
            {
                if (role == 0)
                {
                    result += "@everyone ";
                }
                else
                {
                    result += $"{MentionUtils.MentionRole(role)} ";
                }
            }
            return result;
        }

        /// <summary>
        /// Build the actual embed
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Embed BuildEmbed()
        {
            var eb = new EmbedBuilder();
            switch (Type)
            {
                case RaidType.Aa:
                    eb.Color = Color.Magenta;
                    break;
                case RaidType.So:
                    eb.Color = Color.Green;
                    break;
                case RaidType.Hrc:
                    eb.Color = Color.LightGrey;
                    break;
                case RaidType.Mol:
                    eb.Color = Color.DarkBlue;
                    break;
                case RaidType.Hof:
                    eb.Color = Color.Gold;
                    break;
                case RaidType.As0:
                case RaidType.As1:
                case RaidType.As2:
                    eb.Color = Color.DarkRed;
                    break;
                case RaidType.Cr0:
                case RaidType.Cr1:
                case RaidType.Cr2:
                case RaidType.Cr3:
                    eb.Color = Color.DarkPurple;
                    break;
                case RaidType.Ss:
                case RaidType.SsI:
                case RaidType.SsF:
                case RaidType.SsFI:
                    eb.Color = Color.Teal;
                    break;
                case RaidType.Ka:
                case RaidType.KaV:
                case RaidType.KaY:
                case RaidType.KaVY:
                    eb.Color = Color.DarkRed;
                    break;
                default:
                    throw new ArgumentException("Invalid raid type");
            }

            eb.Title = GenerateHeadline();
            eb.Description = Build();
            return eb.Build();
        }

        /// <summary>
        /// Builds the list of players
        /// </summary>
        /// <returns></returns>
        public string BuildPlayers()
        {
            var players = "";
            var added = new List<AdderPlayer>();
            foreach (var role in AvailableRoles)
            {
                var player = CurrentPlayers.FirstOrDefault(x => x.Role == role && !added.Contains(x));
                var t = RoleRepresentations.GetRepresentation(role);
                if (player == null)
                    players += $"{RoleRepresentations.GetRepresentation(role)}:\n";
                else
                {
                    players += GeneratePlayerString(player);
                    added.Add(player);
                }
            }

            players += "\n";

            return CurrentPlayers.Where(x => IsAlt(x.Role)).Aggregate(players,
                (current, alt) => current + $"{GeneratePlayerString(alt)}");
        }

        /// <summary>
        /// Generate the player's string
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static string GeneratePlayerString(AdderPlayer player)
        {
            return
                $"{RoleRepresentations.GetRepresentation(player.Role)}: {MentionUtils.MentionUser(player.PlayerId)} {player.Emote}\n";
        }
    }
}