using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Yggdrasil.GoogleSpreadsheet
{
    internal class SpreadsheetCollection
    {
        [JsonIgnore]
        internal List<Spreadsheet> Spreadsheets { get; private set; }

        private SpreadsheetCollection()
        {
        }

        internal static SpreadsheetCollection FromJson(string json)
        {
            var spreadsheets = JsonConvert.DeserializeObject<Dictionary<string, Spreadsheet>>(json);

            foreach (var (spreadsheetName, spreadsheet) in spreadsheets)
            {
                spreadsheet.Name = spreadsheetName;

                foreach (var (sheetName, sheet) in spreadsheet.Sheets)
                {
                    sheet.Name = sheetName;
                    sheet.Spreadsheet = spreadsheet;
                }

                spreadsheet.CacheSheetList();
            }

            return new SpreadsheetCollection { Spreadsheets = spreadsheets.Values.ToList() };
        }
    }
}