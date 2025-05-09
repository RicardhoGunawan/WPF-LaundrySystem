using System;
using System.Globalization;
using System.Windows.Data;

namespace Laundry.Converters
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle different numeric types
            decimal amount = 0;
            
            if (value is decimal decimalValue)
                amount = decimalValue;
            else if (value is double doubleValue)
                amount = (decimal)doubleValue;
            else if (value is int intValue)
                amount = intValue;
            else if (value is long longValue)
                amount = longValue;
            else if (value is string stringValue && decimal.TryParse(stringValue, out decimal parsedValue))
                amount = parsedValue;
            else
                return value; // Return original value if we can't convert
            
            // Format parameter handling
            string format = parameter as string ?? "C0";
            
            // If format starts with 'N' or 'C', use it directly
            if (format.StartsWith("N") || format.StartsWith("C"))
            {
                return amount.ToString(format, culture);
            }
            
            // Otherwise format as currency with "Rp" prefix for Indonesian
            if (culture.Name.StartsWith("id"))
            {
                return $"Rp {amount.ToString("N0", culture)}";
            }
            
            // Default formatting
            return amount.ToString("C0", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}