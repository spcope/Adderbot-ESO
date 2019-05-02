using System;
using System.Linq;
using Adderbot.Models;

namespace Adderbot.Helpers
{
    public static class GuildHelper
    {
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