using System.Collections.Generic;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class SheetData
    {
        public string Key { get; set; }

        public Dictionary<string, object> Values { get; set; }

        public List<SheetData> Children { get; set; }
    }
}