using Discord;
using Newtonsoft.Json;

namespace Adderbot.Models
{
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
}