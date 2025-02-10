using System;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class SpreadsheetParserAttribute : Attribute
    {
        public string Id { get; }

        public SpreadsheetParserAttribute(string id)
        {
            Id = id;
        }
    }
}