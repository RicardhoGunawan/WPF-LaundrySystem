using System;
using System.Globalization;
using System.Windows.Data;

namespace Laundry.Converters
{
    public class StringToCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                string caseParam = parameter as string ?? "none";
                
                switch (caseParam.ToLowerInvariant())
                {
                    case "upper":
                        return text.ToUpper(culture);
                    case "lower":
                        return text.ToLower(culture);
                    case "title":
                        TextInfo textInfo = culture.TextInfo;
                        return textInfo.ToTitleCase(text.ToLower(culture));
                    default:
                        return text;
                }
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}