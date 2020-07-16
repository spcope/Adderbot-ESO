using System;
using Adderbot.Models;

namespace Adderbot.Helpers
{
    /// <summary>
    /// Contains helper methods used to work with AdderGuilds
    /// </summary>
    public static class GuildHelper
    {
        /// <summary>
        /// Returns the AdderGuild with the given Id
        /// </summary>
        /// <param name="guildId">The Id of the guild to find</param>
        /// <returns>Returns the AdderGuild whose Id is guildId</returns>
        public static AdderGuild GetGuildById(ulong guildId)
        {
            try
            {
                return Adderbot.Data.Guilds.Find(x => x.GuildId == guildId);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}