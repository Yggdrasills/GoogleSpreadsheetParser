using System;

namespace Yggdrasil.GoogleSpreadsheet
{
    public class SpreadsheetRequestException : Exception
    {
        public SpreadsheetRequestException(string message) : base(message)
        {
        }

        public SpreadsheetRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}