using System.Threading.Tasks;
using Discord.Commands;

namespace Adderbot.Modules
{
    public class MiscModule : BaseModule
    {
        /// <summary>
        /// Handles ;flip
        /// Flips a coin and send Heads if 0 and Tails if 1
        /// </summary>
        /// <returns></returns>
        [Command("flip")]
        [Summary("Flips the coin")]
        public async Task FlipCoinAsync()
        {
            await TimeDeleteMessage(
                Adderbot.randomGen.Next(0, 2) == 0 ? (await ReplyAsync("Heads!")) : (await ReplyAsync("Tails!")),
                10000);
        }
    }
}