using System;
using System.Collections.Generic;
using System.Linq;

namespace Yggdrasil.GoogleSpreadsheet
{
    public abstract class SpreadsheetParserBase : ISpreadsheetParser
    {
        SheetData ISpreadsheetParser.Parse(List<List<object>> data)
        {
            ValidateData(data);
            return ParseInternal(data);
        }

        protected abstract SheetData ParseInternal(List<List<object>> data);

        protected virtual void ValidateData(List<List<object>> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Count < 2) throw new ArgumentException("Data must contain header row and at least one data row");
            if (data[0] == null || !data[0].Any()) throw new ArgumentException("Header row cannot be empty");
        }

        protected virtual Dictionary<string, object> ParseRow(List<object> row, List<object> headers)
        {
            var result = new Dictionary<string, object>();

            for (var i = 0; i < Math.Min(row.Count, headers.Count); i++)
            {
                var header = headers[i]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(header)) continue;

                if (header.EndsWith("[]"))
                {
                    result[header.Replace("[]", "")] = ParseArray(row[i]);
                }
                else
                {
                    result[header] = ParseValue(row[i]);
                }
            }

            return result;
        }

        protected virtual object ParseValue(object value)
        {
            return SpreadsheetsParserUtilities.ParseValue(value);
        }

        protected virtual IEnumerable<object> ParseArray(object value)
        {
            return SpreadsheetsParserUtilities.ParseArray(value);
        }
    }
}