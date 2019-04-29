using Discord.WebSocket;
using System;

namespace Adderbot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Adderbot.StartAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
