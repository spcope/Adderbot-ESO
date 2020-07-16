using Discord;
using Newtonsoft.Json;

namespace Adderbot.Models
{
    /// <summary>
    /// Class used to represent a player in an AdderRaid
    /// </summary>
    public class AdderPlayer
    {
        /// <summary>
        /// Discord Id of the user who has signed up for a raid
        /// </summary>
        [JsonProperty("player")] public ulong PlayerId { get; set; }

        /// <summary>
        /// String representation of the emote used when a person signed up for a raid
        /// </summary>
        [JsonProperty("emote")] public string Emote { get; set; }

        /// <summary>
        /// int representation of the role (generated via the Role enum)
        /// </summary>
        [JsonProperty("role")] public int RoleId { get; set; }

        /// <summary>
        /// Role enum representation of the role the player signed up for
        /// </summary>
        [JsonIgnore] public readonly Role Role;

        /// <summary>
        /// Emote object representation of the role the player signed up for
        /// </summary>
        [JsonIgnore] public readonly Emote EmoteObj;

        /// <summary>
        /// Creates an AdderPlayer with the given:
        /// Discord Id of the user who has signed up for the raid
        /// String representation of the emote used when a person signed up for a raid
        /// int representation of the role the user signed up for
        /// </summary>
        /// <param name="pl">Discord Id of the user who signed up for the raid</param>
        /// <param name="emote">String representation of the emote</param>
        /// <param name="role">int representation of the role</param>
        [JsonConstructor]
        public AdderPlayer(ulong pl, string emote, int role)
        {
            PlayerId = pl;
            Emote = emote;
            // Try to get the emote object
            EmoteObj = emote != null ? Discord.Emote.Parse(emote) : null;
            RoleId = role;
            // Try to get the Role enum
            Role = (Role) role;
        }

        /// <summary>
        /// Creates an AdderPlayer with the given user, role, and emote
        /// </summary>
        /// <param name="pl">Discord Id of the user who signed up for the raid</param>
        /// <param name="r">Role enum representation of the role</param>
        /// <param name="emote">Emote object representation of the role</param>
        public AdderPlayer(ulong pl, Role r, Emote emote)
        {
            PlayerId = pl;
            // Convert role to a hash code (int representation)
            RoleId = r.GetHashCode();
            Role = r;
            EmoteObj = emote;
            // Convert emote to null or its string represenetation
            Emote = emote?.ToString();
        }
    }
}