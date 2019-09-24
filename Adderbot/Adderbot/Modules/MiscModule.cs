using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Adderbot.Modules
{
    internal class MiscModule : BaseModule
    {
        [Command("summon")]
        [Summary("@s the raiders listed in the raid in the channel")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SummonRaiders()
        {
            var raid = GetRaid().Result;
            if (raid != null)
            {
                await ReplyAsync($"{raid.BuildPlayers()}{raid.Type} led by <@{raid.Lead}> is forming. X up in guild or whisper them in game.");
            }
        }
    }
}