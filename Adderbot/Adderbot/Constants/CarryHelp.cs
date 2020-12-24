namespace Adderbot.Constants
{
    public static class CarryHelp
    {
        public const string TotalText = "Total Gold Amount: $";

        public static class TrialHelp
        {
            public const string Trial = "trial";
            public static readonly string[] TrialPayoutTypes = { "Finder's Fee Returning: ", "Payout for Returning: ", "Finder's Fee New: ", "Payout for New: " };
            public static readonly double[] TrialModifiers = { .05, .08636363, .08, .08363636 };
        }
        
        public static class FourManHelp
        {
            public const string FourMan = "4man";
            public const string Arena = "arena";
            public const string Dungeon = "dungeon";
            public static readonly string[] FourManPayoutTypes = { "Finder's Fee: ", "Payout for 3 Man: ", "Payout for 4 Man: " };
            public static readonly double[] FourManModifiers = { .1, .3, .225 };
        }
    }
}