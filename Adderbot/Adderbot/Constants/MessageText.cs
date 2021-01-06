namespace Adderbot.Constants
{
    public static class MessageText
    {
        /// <summary>
        /// Contains standard error messages used by the bot
        /// </summary>
        public static class Error
        {
            public const string InvalidGuild = "Somehow the guild could not be found, contact the developers for help";
            public const string InvalidRaid = "Adderbot could not find a raid in that channel";
            public const string InvalidRole = "Please enter a valid role!";
            public const string NotRaidLead = "You are not the lead for this raid and cannot add allowed roles";
            public const string HasNoRaidLeadRole = "You do not have permission to create raids";

            public const string CommandUnavailable =
                "This command is unavailable to you. Please contact the developers for more information.";

            public const string RaidNotFoundForLead =
                "There are either no raids, or no raids led by you in the channel!";

            public const string EmoteUnparseable =
                "Emote could not be parsed. You'll still be added to the raid, but without the emote";

            public const string EmoteInvalid =
                "Emote not available. You'll still be added to the raid, but without the emote";

            public const string AlreadyInRaid =
                "You already joined this raid, if you want to change your role, remove yourself from it and add yourself again.";

            public const string CommandBadArgCount =
                "Not enough arguments for that command were provided. Use the help command to check out the commands and their arguments.";

            public const string CommandInvalid =
                "Command could not be found. Use the help command to check out the commands.";

            public const string EmotesUnavailable = "Emotes have not been enabled for this server.";

            public const string EmotesCannotBeAdded =
                "Could not add all the emotes, typically this means you do not have enough space. At least 24 slots are required to add all emotes.";

            public const string EmotesCannotBeDeleted = "Could not delete all the emotes.";

            public const string InvalidTime =
                "Time was in invalid time format. Please use 12-hour time with am/pm attached. For example 2pm or 7am";

            public const string InvalidTimezone = "Timezone is invalid. Please use a standard timezone.";
            public const string NotCorrectGoldFormat = "The amount of gold you provided is not a double value";
        }

        public static class Success
        {
            public const string EmoteAddSuccessful = "Successfully added emotes!";

            public const string EmoteDeleteSuccessful = "Successfully deleted emotes!";
        }

        public static class Misc
        {
            public const string DeleteSuccessful = "Deleted raid successfully!";

            public const string LongCommandWarning =
                "This command takes a long time to complete, Adderbot will message you when it is complete.";
        }
    }
}