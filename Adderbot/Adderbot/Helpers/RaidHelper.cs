using System;
using System.Linq;
using Adderbot.Models;

namespace Adderbot.Helpers
{
    /// <summary>
    /// Contains helper methods used to work with AdderRaids
    /// </summary>
    public static class RaidHelper
    {
        /// <summary>
        /// Returns the AdderRaid associated with both the AdderGuild and the channel in that guild
        /// </summary>
        /// <param name="adderGuild">The guild object to find the raid from</param>
        /// <param name="channelId">The id of the text channel with the raid in it</param>
        /// <returns></returns>
        public static AdderRaid GetRaidByChannelId(AdderGuild adderGuild, ulong channelId)
        {
            try
            {
                return adderGuild.ActiveRaids.Find(x => x.ChannelId == channelId).Raid;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Verifies that the passed in user is the lead of the passed in raid
        /// </summary>
        /// <param name="uid">The id of user to validate</param>
        /// <param name="raid">The AdderRaid to verify against</param>
        /// <returns></returns>
        public static bool CheckUserIsLead(ulong uid, AdderRaid raid)
        {
            return uid == raid.Lead;
        }

        public static void RemoveRaid(AdderGuild guild, ulong channelId, ulong userId)
        {
            guild.ActiveRaids.Remove(guild.ActiveRaids.FirstOrDefault(x =>
                (x.ChannelId == channelId && x.Raid?.Lead == userId)));
        }
    }
}