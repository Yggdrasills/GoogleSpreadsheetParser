using System;
using System.Collections.Generic;
using System.Linq;

namespace Yggdrasil.GoogleSpreadsheet
{
    public static class SpreadsheetsParserUtilities
    {
        public static IReadOnlyList<object> ParseArray(object value)
        {
            var valueStr = value.ToString();

            if (string.IsNullOrWhiteSpace(valueStr))
                return Array.Empty<object>();

            return valueStr
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => ParseValue(v.Trim()))
                .ToList()
                .AsReadOnly();
        }

        public static object ParseValue(object value)
        {
            var valueStr = value.ToString().Trim();

            if (string.IsNullOrWhiteSpace(valueStr))
                return string.Empty;

            if (bool.TryParse(valueStr, out var boolResult))
                return boolResult;

            if (int.TryParse(valueStr, out var intResult))
                return intResult;

            if (float.TryParse(valueStr, out var floatResult))
                return floatResult;

            return valueStr;
        }
    }
}