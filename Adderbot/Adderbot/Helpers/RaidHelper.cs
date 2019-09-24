using System;
using System.Linq;
using Adderbot.Models;

namespace Adderbot.Helpers
{
    public static class RaidHelper
    {
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
    }
}