using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    [SpreadsheetParser("complex")]
    public class ComplexSpreadsheetParser : SpreadsheetParserBase
    {
        protected override SheetData ParseInternal(List<List<object>> data)
        {
            var root = new SheetData { Children = new List<SheetData>() };
            SheetData currentCategory = null;

            for (var i = 1; i < data.Count; i++)
            {
                var row = data[i];

                if (row == null || !row.Any())
                    continue;

                var categoryName = row[0]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(categoryName))
                {
                    currentCategory = new SheetData
                    {
                        Key = categoryName,
                        Children = new List<SheetData>()
                    };
                    root.Children.Add(currentCategory);
                    continue;
                }

                currentCategory?.Children.Add(new SheetData
                {
                    Values = ParseRow(row.Skip(1).ToList(), data[0].Skip(1).ToList())
                });
            }

            return root;
        }
    }
}