using Newtonsoft.Json;

namespace Adderbot.Models
{
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
}