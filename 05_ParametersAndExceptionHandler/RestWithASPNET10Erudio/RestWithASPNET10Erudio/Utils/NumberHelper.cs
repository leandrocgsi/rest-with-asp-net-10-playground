using System.Globalization;

namespace RestWithASPNET10Erudio.Utils
{
    public static class NumberHelper
    {
        public static bool IsNumeric(string strNumber)
        {
            double number;
            return double.TryParse(
                strNumber,
                NumberStyles.Any,
                NumberFormatInfo.InvariantInfo,
                out number
            );
        }

        public static decimal ConvertToDecimal(string strNumber)
        {
            if (decimal.TryParse(strNumber, out var decimalValue))
                return decimalValue;

            return 0;
        }
    }
}