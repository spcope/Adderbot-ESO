using System.Collections.Generic;
using Newtonsoft.Json;

namespace Adderbot.Models
{
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
        
        public static AdderData FromJson(string json) =>
            JsonConvert.DeserializeObject<AdderData>(json, Converter.Settings);
    }
}