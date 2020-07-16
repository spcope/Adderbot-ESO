using Newtonsoft.Json;

namespace Adderbot.Models
{
    /// <summary>
    /// Class used to document a channel and its associated raid
    /// </summary>
    public class AdderChannel
    {
        /// <summary>
        /// Discord Id of the channel the raid is in
        /// </summary>
        [JsonProperty("ChannelId")] public ulong ChannelId { get; set; }

        /// <summary>
        /// The raid that resides in the channel specified by ChannelId
        /// </summary>
        [JsonProperty("Raid")] public AdderRaid Raid { get; set; }

        /// <summary>
        /// Creates an AdderChannel with only a Discord channel id
        /// </summary>
        /// <param name="cid">Discord Id of the channel</param>
        public AdderChannel(ulong cid)
        {
            ChannelId = cid;
            Raid = null;
        }

        /// <summary>
        /// Creates an AdderChannel with both a Discord channel id and an already created AdderRaid
        /// </summary>
        /// <param name="cid">Discord Id of the channel</param>
        /// <param name="ar">AdderRaid that resides in the channel specified by cid</param>
        [JsonConstructor]
        public AdderChannel(ulong cid, AdderRaid ar)
        {
            ChannelId = cid;
            Raid = ar;
        }
    }
}