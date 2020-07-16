using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Adderbot.Constants;
using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Adderbot.Models
{
    /// <summary>
    /// Class used to serialize the AdderData object into a JSON string
    /// </summary>
    public static class Serialize
    {
        /// <summary>
        /// Converts the AdderData object into a JSON string representation
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToJson(this AdderData self) =>
            JsonConvert.SerializeObject(self, Converter.Settings);
    }

    /// <summary>
    /// Class used to setup the converter
    /// </summary>
    internal static class Converter
    {
        /// <summary>
        /// Settings the converter uses. Very standard setup.
        /// </summary>
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            }
        };
    }
}