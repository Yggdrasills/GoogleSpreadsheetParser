using Newtonsoft.Json;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class Sheet
    {
        [JsonIgnore]
        internal Spreadsheet Spreadsheet { get; set; }

        [JsonIgnore]
        internal string Name { get; set; }

        [JsonProperty]
        internal string Type { get; set; }
    }
}