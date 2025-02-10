using System.Collections.Generic;
using Newtonsoft.Json;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class Spreadsheet
    {
        [JsonIgnore]
        internal string Name { get; set; }

        [JsonIgnore]
        internal List<Sheet> SheetList { get; private set; }

        [JsonProperty("spreadsheet_id")]
        internal string SpreadsheetId { get; set; }

        [JsonProperty]
        internal Dictionary<string, Sheet> Sheets { get; set; }

        internal void CacheSheetList()
        {
            SheetList = new List<Sheet>(Sheets.Values);
        }
    }
}