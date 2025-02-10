using System.Collections.Generic;

namespace Yggdrasil.GoogleSpreadsheet
{
    public interface ISpreadsheetParser
    {
        SheetData Parse(List<List<object>> sheetData);
    }
}