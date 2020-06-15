namespace Adderbot.Constants
{
    public static class MessageText
    {
        public static class Error
        {
            public const string InvalidGuild = "Somehow the guild could not be found, contact the developers for help";
            public const string InvalidRaid = "Adderbot could not find a raid in that channel";
            public const string NotRaidLead = "You are not the lead for this raid and cannot add allowed roles";
            public const string HasNoRaidLeadRole = "You do not have permission to create raids";
        }
    }
}