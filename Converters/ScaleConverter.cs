using System;
using System.Globalization;
using System.Windows.Data;

namespace KoreanRailwayTrackEditor.Converters
{
    public class ScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && parameter is string s && double.TryParse(s, out double scale))
            {
                return d * scale;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
