using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    [SpreadsheetParser("default")]
    public class DefaultSpreadsheetParser : SpreadsheetParserBase
    {
        protected override SheetData ParseInternal(List<List<object>> rows)
        {
            var root = new SheetData { Children = new List<SheetData>() };

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];

                if (row == null || !row.Any())
                    continue;

                var key = row[0].ToString();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                root.Children.Add(new SheetData
                {
                    Key = key,
                    Values = ParseRow(row.Skip(1).ToList(), rows[0].Skip(1).ToList())
                });
            }

            return root;
        }
    }
}