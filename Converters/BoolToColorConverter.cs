using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Laundry.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Default colors
                Color trueColor = Color.FromRgb(76, 175, 80);  // Green
                Color falseColor = Color.FromRgb(244, 67, 54); // Red
                
                // Allow customization via parameter
                if (parameter is string paramStr && paramStr.Contains("|"))
                {
                    var parts = paramStr.Split('|');
                    if (parts.Length >= 2)
                    {
                        try
                        {
                            // Try to parse hex colors (#RRGGBB format)
                            if (parts[0].StartsWith("#") && parts[0].Length == 7)
                                trueColor = (Color)ColorConverter.ConvertFromString(parts[0]);
                            
                            if (parts[1].StartsWith("#") && parts[1].Length == 7)
                                falseColor = (Color)ColorConverter.ConvertFromString(parts[1]);
                        }
                        catch { /* Use default colors if parsing fails */ }
                    }
                }
                
                return new SolidColorBrush(boolValue ? trueColor : falseColor);
            }
            
            // Default to gray for null or invalid values
            return new SolidColorBrush(Color.FromRgb(158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}