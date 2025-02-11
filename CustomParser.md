Creating Custom Parser
===

### Overview
You can create custom parsers by implementing the `ISpreadsheetParser` interface. For most cases, it's recommended to inherit from `SpreadsheetParserBase` which provides common parsing utilities

### Implementation Steps
- Create a new class that inherits from `SpreadsheetParserBase`:
```
using Yggdrasil.GoogleSpreadsheet;

[SpreadsheetParser("custom")]
public class CustomSpreadsheetParser : SpreadsheetParserBase
{
  protected override SheetData ParseInternal(List<List<object>> data)
  {
      // Your parsing logic here
  }
}
```
- Add the `SpreadsheetParserAttribute` with a unique identifier that will be used in the `index.json` file.
- Override the ParseInternal method to implement your parsing logic. The method receives the raw spreadsheet data as a list of lists, where:
  - data[0] contains the header row
  - data[1] onwards contains the data rows
  - Each row is a List<object> containing cell values
- Optionally override ValidateData to add custom validation:
```
protected override void ValidateData(List<List<object>> data)
{
    base.ValidateData(data);  // Call base validation first
    
    // Add custom validation
    if (data[0].Count < 3)
        throw new ArgumentException("Sheet must contain at least three columns");
}
```

### Available Base Class Utilities
The `SpreadsheetParserBase` provides several helpful methods:

- `ParseRow(List<object> row, List<object> headers):` Converts a row into a dictionary using header names as keys
- `ParseValue(object value):` Automatically detects and converts value types (numbers, booleans, strings)
- `ParseArray(object value):` Parses comma-separated values into arrays

### Example Custom Parser
Here's an example of a custom parser that groups items by multiple categories:

```
[SpreadsheetParser("multi_category")]
public class MultiCategorySpreadsheetParser : SpreadsheetParserBase
{
    protected override SheetData ParseInternal(List<List<object>> data)
    {
        var root = new SheetData { Children = new List<SheetData>() };
        
        // Skip header row
        for (var i = 1; i < data.Count; i++)
        {
            var row = data[i];
            if (row == null || row.Count < 3) continue;
            
            var category1 = row[0]?.ToString();
            var category2 = row[1]?.ToString();
            
            if (string.IsNullOrEmpty(category1) || string.IsNullOrEmpty(category2))
                continue;
                
            // Create nested structure
            var cat1Node = root.Children.FirstOrDefault(x => x.Key == category1);
            if (cat1Node == null)
            {
                cat1Node = new SheetData 
                { 
                    Key = category1,
                    Children = new List<SheetData>()
                };
                root.Children.Add(cat1Node);
            }
            
            var cat2Node = cat1Node.Children.FirstOrDefault(x => x.Key == category2);
            if (cat2Node == null)
            {
                cat2Node = new SheetData
                {
                    Key = category2,
                    Children = new List<SheetData>()
                };
                cat1Node.Children.Add(cat2Node);
            }
            
            // Add item data
            cat2Node.Children.Add(new SheetData
            {
                Values = ParseRow(row.Skip(2).ToList(), data[0].Skip(2).ToList())
            });
        }
        
        return root;
    }
}
```

### Using Custom Parsers
- Add your parser class to your project
- Reference it in your index.json using the identifier specified in the SpreadsheetParserAttribute:
```
{
  "MySpreadsheet": {
    "spreadsheet_id": "...",
    "sheets": {
      "CustomSheet": {
        "type": "custom"
      }
    }
  }
}
```
