using System;
using System.Globalization;
using System.Windows.Data;

namespace KoreanRailwayTrackEditor.Converters
{
    public class MidpointRelativeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double v1 && values[1] is double v2 && values[2] is double baseVal)
            {
                // Calculate midpoint between v1 and v2, then subtract baseVal to get relative position
                return (v1 + v2) / 2.0 - baseVal;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
