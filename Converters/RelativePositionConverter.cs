using System;
using System.Globalization;
using System.Windows.Data;

namespace KoreanRailwayTrackEditor.Converters
{
    public class RelativePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double end && values[1] is double start)
            {
                double ratio = 1.0;
                if (parameter is string s && double.TryParse(s, out double r))
                {
                    ratio = r;
                }
                return (end - start) * ratio;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
