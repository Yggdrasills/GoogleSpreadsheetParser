using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    [SpreadsheetParser("key_value")]
    public class KeyValueSpreadsheetParser : SpreadsheetParserBase
    {
        protected override SheetData ParseInternal(List<List<object>> data)
        {
            var root = new SheetData();
            root.Values = new Dictionary<string, object>();

            for (var i = 1; i < data.Count; i++)
            {
                var row = data[i];

                if (row == null || row.Count < 2 || row[0] == null)
                    continue;

                var key = row[0].ToString()?.Trim();

                if (string.IsNullOrEmpty(key))
                    continue;

                root.Values[key] = ParseValue(row[1]);
            }

            return root;
        }

        protected override void ValidateData(List<List<object>> data)
        {
            base.ValidateData(data);

            if (data[0].Count < 2)
                throw new ArgumentException("Sheet must contain at least two columns: key and value");

            var headerKey = data[0][0]?.ToString()?.Trim().ToLower();
            var headerValue = data[0][1]?.ToString()?.Trim().ToLower();

            var isValidKey = headerKey is "key" or "id";

            if (!isValidKey || headerValue != "value")
                throw new ArgumentException("First row must contain 'key' and 'value' headers");
        }
    }
}