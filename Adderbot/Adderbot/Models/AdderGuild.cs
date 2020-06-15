using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adderbot.Models
{
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
}