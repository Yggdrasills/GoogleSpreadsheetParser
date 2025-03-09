using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    [SpreadsheetParser("complex")]
    public class ComplexSpreadsheetParser : SpreadsheetParserBase
    {
        protected override SheetData ParseInternal(List<List<object>> rows)
        {
            var root = new SheetData { Values = new Dictionary<string, object>() };
            string currentCategory = null;
            List<Dictionary<string, object>> currentCategoryItems = null;

            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];

                if (row.All(cell => string.IsNullOrWhiteSpace(cell.ToString())))
                    continue;

                var firstCell = row[0]?.ToString()?.Trim();

                if (!string.IsNullOrEmpty(firstCell))
                {
                    currentCategory = firstCell;
                    currentCategoryItems = new List<Dictionary<string, object>>();
                    root.Values[currentCategory] = currentCategoryItems;

                    if (row.Count > 1)
                    {
                        var rowValues = ParseRow(row.Skip(1).ToList(), rows[0].Skip(1).ToList());
                        if (rowValues.Count > 0)
                        {
                            currentCategoryItems.Add(rowValues);
                        }
                    }

                    continue;
                }

                if (currentCategory != null)
                {
                    var rowValues = ParseRow(row.Skip(1).ToList(), rows[0].Skip(1).ToList());
                    if (rowValues.Count > 0)
                    {
                        currentCategoryItems.Add(rowValues);
                    }
                }
            }

            return root;
        }
    }
}