using System;
using System.Collections.Generic;

namespace Yggdrasil.GoogleSpreadsheet
{
    public abstract class SpreadsheetParserBase : ISpreadsheetParser
    {
        SheetData ISpreadsheetParser.Parse(List<List<object>> rows)
        {
            ValidateData(rows);
            return ParseInternal(rows);
        }

        protected abstract SheetData ParseInternal(List<List<object>> rows);

        protected virtual void ValidateData(List<List<object>> rows)
        {
            if (rows.Count < 2)
                throw new ArgumentException("Data must contain header row and at least one data row");

            var headerRow = rows[0];
            for (var index = 0; index < headerRow.Count; index++)
            {
                if (string.IsNullOrWhiteSpace(headerRow[index].ToString()))
                {
                    throw new ArgumentException("Header cell by index " + index + " cannot be empty");
                }
            }
        }

        protected virtual Dictionary<string, object> ParseRow(List<object> row, List<object> headers)
        {
            var result = new Dictionary<string, object>();

            var rowLenght = Math.Min(row.Count, headers.Count);

            for (var i = 0; i < rowLenght; i++)
            {
                var header = headers[i]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(header))
                    continue;

                if (header.EndsWith("[]"))
                {
                    var value = ParseArray(row[i]);
                    if (value.Count > 0)
                        result[header.Substring(0, header.Length - 2)] = value;
                }
                else
                {
                    var value = ParseValue(row[i]);
                    if (!string.IsNullOrEmpty(value.ToString()))
                        result[header] = value;
                }
            }

            return result;
        }

        protected virtual object ParseValue(object value)
        {
            return SpreadsheetsParserUtilities.ParseValue(value);
        }

        protected virtual IReadOnlyList<object> ParseArray(object value)
        {
            return SpreadsheetsParserUtilities.ParseArray(value);
        }
    }
}