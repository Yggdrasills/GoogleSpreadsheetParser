using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Yggdrasil.GoogleSpreadsheet
{
    [UsedImplicitly]
    [SpreadsheetParser("index-based")]
    public class IndexBasedSpreadsheetParser : SpreadsheetParserBase
    {
        protected override SheetData ParseInternal(List<List<object>> rows)
        {
            var root = new SheetData { Children = new List<SheetData>() };
            
            for (var i = 1; i < rows.Count; i++)
            {
                var row = rows[i];

                if (row == null || !row.Any())
                    continue;

                var key = (i - 1).ToString();
                
                root.Children.Add(new SheetData
                {
                    Key = key,
                    Values = ParseRow(row.ToList(), rows[0].ToList())
                });
            }

            return root;
        }
    }
    
}