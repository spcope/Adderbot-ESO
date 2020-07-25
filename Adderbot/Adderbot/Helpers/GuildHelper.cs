using System;
using System.Collections.Generic;
using System.Linq;
using Adderbot.Models;
using Discord.WebSocket;

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

        /// <summary>
        /// Determine if the user is an admin in the guild
        /// </summary>
        /// <param name="g">SocketGuild of the guild to validate</param>
        /// <param name="u">SocketUser of the user to check</param>
        /// <returns>True if user is an admin; false if user is not an admin</returns>
        public static bool IsUserAdminInGuild(SocketGuild g, SocketUser u)
        {
            if (g == null || u == null) return false;
            var guildUser = g.Users.FirstOrDefault(x => x.Id == u.Id);
            return guildUser != null && guildUser.GuildPermissions.Administrator;
        }

        /// <summary>
        /// Determine if user can edit a raid. This is determined by both Admin permissions and raid lead status
        /// </summary>
        /// <param name="g">SocketGuild of the guild to validate</param>
        /// <param name="u">SocketUser of the user to check</param>
        /// <param name="r">AdderRaid of the raid to validate against</param>
        /// <returns>True if the user can edit raid; false if the user cannot</returns>
        public static bool IsUserRaidEditor(SocketGuild g, SocketUser u, AdderRaid r)
        {
            return RaidHelper.CheckUserIsLead(u.Id, r) || IsUserAdminInGuild(g, u);
        }

        public static List<AdderChannel> FindRaidsUserIsIn(SocketGuild guild, ulong userId)
        {
            return GetGuildById(guild.Id).ActiveRaids
                .Where(x => x.Raid.CurrentPlayers.Any(y => y.PlayerId == userId)).ToList();
        }
    }
}