using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adderbot.Models
{
    /// <summary>
    /// Class used to store information about a Discord guild (server)
    /// </summary>
    public class AdderGuild
    {
        /// <summary>
        /// Discord Id of the guild (server)
        /// </summary>
        [JsonProperty("GuildId")] public ulong GuildId { get; set; }
        
        /// <summary>
        /// Flag showing if emotes are available in the Discord guild specified by GuildId
        /// </summary>
        [JsonProperty("EmotesAvailable")] public bool EmotesAvailable { get; set; }

        /// <summary>
        /// Discord Id of the role that the guild uses to create raids
        /// </summary>
        [JsonProperty("TrialLead")] public ulong Lead { get; set; }

        /// <summary>
        /// A list of AdderChannels that represents the active raids in the Discord guild (server)
        /// </summary>
        [JsonProperty("ActiveRaids")] public SynchronizedCollection<AdderChannel> ActiveRaids { get; set; }

        /// <summary>
        /// Creates an empty AdderGuild with the given Discord Id of the guild (server)
        /// </summary>
        /// <param name="gid">Discord Id of the guild (server)</param>
        public AdderGuild(ulong gid)
        {
            GuildId = gid;
            Lead = 0;
            ActiveRaids = new SynchronizedCollection<AdderChannel>();
        }

        /// <summary>
        /// Creates an AdderGuild with the given Discord Id of the guild (server) and Id of the role used to create raids
        /// </summary>
        /// <param name="gid">Discord Id of the guild (server)</param>
        /// <param name="lead">Discord Id of the role used to create raids</param>
        public AdderGuild(ulong gid, ulong lead)
        {
            GuildId = gid;
            Lead = lead;
            ActiveRaids = new SynchronizedCollection<AdderChannel>();
        }

        /// <summary>
        /// Creates an AdderGuild with the given:
        /// Discord Id of the guild (server)
        /// Flag of emote availability
        /// Discord Id of the role used to create raids
        /// List of AdderChannels specifying active raids in the Discord guild (server)
        /// </summary>
        /// <param name="gid">Discord Id of the guild (server)</param>
        /// <param name="emotesAvailable">Flag representing emote availability</param>
        /// <param name="lead">Discord Id of the role used to create raids</param>
        /// <param name="ars">List of AdderChannels representing active raids in the Discord guild (server)</param>
        [JsonConstructor]
        public AdderGuild(ulong gid, bool emotesAvailable, ulong lead, SynchronizedCollection<AdderChannel> ars)
        {
            GuildId = gid;
            EmotesAvailable = emotesAvailable;
            Lead = lead;
            ActiveRaids = ars;
        }
    }
}