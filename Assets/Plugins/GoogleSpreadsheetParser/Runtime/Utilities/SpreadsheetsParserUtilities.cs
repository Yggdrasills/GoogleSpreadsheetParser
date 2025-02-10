using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Yggdrasil.GoogleSpreadsheet
{
    public static class SpreadsheetsParserUtilities
    {
        private static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture.NumberFormat;

        public static IReadOnlyList<object> ParseArray(object value)
        {
            if (value == null)
                return Array.Empty<object>();

            var valueStr = value.ToString();

            if (string.IsNullOrEmpty(valueStr))
                return Array.Empty<object>();

            return valueStr
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => ParseValue(v.Trim()))
                .ToList()
                .AsReadOnly();
        }

        public static object ParseValue(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var valueStr = value.ToString().Trim();

            if (string.IsNullOrEmpty(valueStr))
                return string.Empty;

            if (bool.TryParse(valueStr, out var boolResult))
                return boolResult;

            if (int.TryParse(valueStr, NumberStyles.Integer, FormatProvider, out var intResult))
                return intResult;

            if (float.TryParse(valueStr, NumberStyles.Float, FormatProvider, out var floatResult))
                return floatResult;

            return valueStr;
        }
    }
}