using System;
using System.Globalization;
using System.Windows.Data;

namespace Laundry.Converters
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Memungkinkan untuk kustomisasi pesan melalui parameter
                string trueText = parameter as string ?? "Lunas";
                string falseText = "Belum Dibayar";
                
                // Jika parameter berisi pemisah pipe, kita bisa mendefinisikan kedua pesan
                if (parameter is string paramStr && paramStr.Contains("|"))
                {
                    var parts = paramStr.Split('|');
                    if (parts.Length >= 2)
                    {
                        trueText = parts[0];
                        falseText = parts[1];
                    }
                }
                
                return boolValue ? trueText : falseText;
            }
            return "Belum Dibayar";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}