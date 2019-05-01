namespace Adderbot.Constants
{
    public static class CommandHelp
    {
        public static class AdminCommandHelp
        {
            public const string SetLeadRole = "`;setleadrole <user role>` - sets the trial lead role for the guild.";

            public const string Purge =
                "`;purge` - clears the past 100 messages from the channel where the command is issued.";
            
            public static readonly string Representation = $":snake:  __**Admin Commands**__  :snake:\n\n:large_blue_diamond: {SetLeadRole}\n\n" +
                       $":large_orange_diamond: {Purge}\n\n";
        }

        public static class RaidLeadCommandHelp
        {
            public const string Create = "`;create <trial difficulty> <trial acronym> " +
                                         "<date> <time> <timezone> <disallow melee (optional)> <allowed roles separated by | (optional)>` - " +
                                         "creates a trial with the given properties. Note if adding prog roles, one must specifiy the disallow" +
                                         " melee flag's value.";

            public const string AddAllowedRole = "`;addallowedrole <role name>` - adds the named role to the list of " +
                                                 "allowed roles for the raid. Note, this will remove the @everyone role.";

            public const string RemoveAllowedRole = "`;removeallowedrole <role name>` - removes the named role from " +
                                                    "the list of allowed roles for the raid. Note, if all roles are delted @everyone will be addded.";

            public const string Delete = "`;delete` - deletes the raid owned by you in the current channel.";

            public const string RaidRemove =
                "`;raid-remove <user mention>` - removes the specified user from the raid. " +
                "Note, you must mention the user in order for this command to work.";

            public const string RaidAdd = "`;raid-add <user mention> <role>` - adds the specified user to the raid " +
                                          "with the specified role. Note, use abbreviations for the roles.";

            public static readonly string Representation =
                $":fleur_de_lis:  __**Raid Lead Commands**__  :fleur_de_lis:\n\n:large_blue_diamond: {Create}\n\n" +
                $":large_orange_diamond: {AddAllowedRole}\n\n:large_blue_diamond: {Delete}\n\n" +
                $":large_orange_diamond: {RaidAdd}\n\n:large_blue_diamond: {RaidRemove}\n\n" +
                $":large_orange_diamond: {RemoveAllowedRole}\n\n";
        }


        public static class BasicCommandHelp
        {
            public const string Tank = "`;<t | tank>` - signs the user up as a tank";
            public const string OffTank = "`;<ot | offtank>` - signs the user up as an offtank";
            public const string OffTank2 = "`;<ot2 | offtank2>` - signs the user up as the second offtank";
            public const string Healer = "`;<h | healer>` - signs the user up as a healer";

            public const string HealerNumber =
                "`;<h1 | healer1 | h2 | healer2>` - signs the user up as healer in the specified position";

            public const string KiteHealer = "`;<kh | kitehealer>` - signs the user up as the kite healer";
            public const string GroupHealer = "`;<gh | grouphealer>` - signs the user up as the group healer";
            public const string Melee = "`;<m | melee | mdps>` - signs the user up as a melee dps.";

            public const string Ranged = "`;<r | ranged | rdps>` - signs the user up as a ranged dps.";

            public const string Remove = "`;remove` - removes the user from the raid in the current channel";

            public static readonly string Representation =
                $":droplet:  __**Basic Commands**__  :droplet:\n\n:large_blue_diamond: {Tank}\n\n" +
                $":large_orange_diamond: {OffTank}\n\n:large_blue_diamond: {OffTank2}\n\n" +
                $":large_orange_diamond: {Healer}\n\n:large_blue_diamond: {HealerNumber}\n\n" +
                $":large_orange_diamond: {KiteHealer}\n\n:large_blue_diamond: {GroupHealer}\n\n" +
                $":large_orange_diamond: {Melee}\n\n" +
                $":large_orange_diamond: {Ranged}\n\n" +
                $":large_orange_diamond: {Remove}\n\n";
        }

        public static readonly string FullRepresentation = AdminCommandHelp.Representation + RaidLeadCommandHelp.Representation + BasicCommandHelp.Representation;
    }
}