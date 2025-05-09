using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;

namespace Laundry.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        // Define a dictionary for status -> color mapping
        private static readonly Dictionary<string, Color> StatusColors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "Pending", Color.FromRgb(255, 152, 0) },   // Orange
            { "Proses", Color.FromRgb(33, 150, 243) },   // Blue
            { "Diproses", Color.FromRgb(33, 150, 243) }, // Blue (alias)
            { "Dicuci", Color.FromRgb(3, 169, 244) },    // Light Blue
            { "Dikeringkan", Color.FromRgb(0, 188, 212) },// Cyan
            { "Disetrika", Color.FromRgb(0, 150, 136) }, // Teal
            { "Selesai", Color.FromRgb(76, 175, 80) },   // Green
            { "Diambil", Color.FromRgb(139, 195, 74) },  // Light Green
            { "Dibatalkan", Color.FromRgb(244, 67, 54) }, // Red
            { "Terlambat", Color.FromRgb(233, 30, 99) }  // Pink
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status && !string.IsNullOrEmpty(status))
            {
                // Check if we have a predefined color for this status
                if (StatusColors.TryGetValue(status, out Color color))
                {
                    return new SolidColorBrush(color);
                }
                
                // If we have a parameter that has format "status1:color1,status2:color2,..."
                if (parameter is string paramStr)
                {
                    var customMappings = paramStr.Split(',');
                    foreach (var mapping in customMappings)
                    {
                        var parts = mapping.Split(':');
                        if (parts.Length == 2 && parts[0].Trim().Equals(status, StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                // Parse the color from hex
                                if (parts[1].Trim().StartsWith("#"))
                                {
                                    Color customColor = (Color)ColorConverter.ConvertFromString(parts[1].Trim());
                                    return new SolidColorBrush(customColor);
                                }
                            }
                            catch { /* Use default color if parsing fails */ }
                        }
                    }
                }
            }
            
            // Default to gray for unrecognized status or null/empty value
            return new SolidColorBrush(Color.FromRgb(158, 158, 158));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}