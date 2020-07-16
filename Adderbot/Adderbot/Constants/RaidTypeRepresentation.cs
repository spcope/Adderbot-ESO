using System.Collections.Generic;
using Adderbot.Models;

namespace Adderbot.Constants
{
    /// <summary>
    /// Contains representations of the various raid types used by the bot
    /// </summary>
    public static class RaidTypeRepresentation
    {
        private const string Aa = "AA";
        private const string So = "SO";
        private const string Hrc = "HRC";
        private const string Mol = "MoL";
        private const string Hof = "HoF";
        private const string Cr0 = "CR+0";
        private const string Cr1 = "CR+1";
        private const string Cr2 = "CR+2";
        private const string Cr3 = "CR+3";
        private const string As0 = "AS+0";
        private const string As1 = "AS+1";
        private const string As2 = "AS+2";
        private const string Ss = "SS";
        private const string SsI = "SS Ice";
        private const string SsF = "SS Fire";
        private const string SsFI = "SS Fire+Ice";
        private const string Ka = "KA";
        private const string KaV = "Ka Vrol";
        private const string KaY = "KA Yandir";
        private const string KaVY = "KA Vrol+Yandir";

        /// <summary>
        /// Map of RaidType to its textual representation
        /// </summary>
        private static readonly Dictionary<RaidType, string> RaidTypeToRepresentation =
            new Dictionary<RaidType, string>()
            {
                {RaidType.Aa, Aa}, {RaidType.So, So}, {RaidType.Hrc, Hrc}, {RaidType.Mol, Mol}, {RaidType.Hof, Hof},
                {RaidType.Cr0, Cr0}, {RaidType.Cr1, Cr1}, {RaidType.Cr2, Cr2}, {RaidType.Cr3, Cr3}, {RaidType.As0, As0},
                {RaidType.As1, As1}, {RaidType.As2, As2}, {RaidType.Ss, Ss}, {RaidType.SsI, SsI}, {RaidType.SsF, SsF},
                {RaidType.SsFI, SsFI}, {RaidType.Ka, Ka}, {RaidType.KaV, KaV}, {RaidType.KaY, KaY},
                {RaidType.KaVY, KaVY}
            };

        /// <summary>
        /// Gets the representation for the passed in raid type
        /// </summary>
        /// <param name="raidType">Raid type to get the representation for</param>
        /// <returns>String representation of the raid type</returns>
        public static string GetRepresentation(RaidType raidType)
        {
            return RaidTypeToRepresentation.GetValueOrDefault(raidType);
        }
    }
}