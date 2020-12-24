using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adderbot.Models
{
    /// <summary>
    /// Class used to store all data in a single object. Mostly used for JSON serialization ease
    /// </summary>
    public class AdderData
    {
        /// <summary>
        /// List of AdderGuilds that Adderbot manages
        /// </summary>
        [JsonProperty("Guilds")] public SynchronizedCollection<AdderGuild> Guilds { get; set; }

        /// <summary>
        /// Creates an empty AdderData object, with an empty list of AdderGuilds
        /// </summary>
        public AdderData()
        {
            Guilds = new SynchronizedCollection<AdderGuild>();
        }

        /// <summary>
        /// Creates an AdderData object with the passed list of AdderGuilds
        /// </summary>
        /// <param name="guilds">List of AdderGuilds to initialize the AdderData object with</param>
        [JsonConstructor]
        public AdderData(SynchronizedCollection<AdderGuild> guilds)
        {
            Guilds = guilds;
        }
        
        /// <summary>
        /// JSON converter. Converts from the passed in JSON string to an AdderData object
        /// </summary>
        /// <param name="json">The JSON string to parse into an AdderData object</param>
        /// <returns></returns>
        public static AdderData FromJson(string json) =>
            JsonConvert.DeserializeObject<AdderData>(json, Converter.Settings);
    }
}