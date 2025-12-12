using System;
using System.Globalization;
using System.Windows.Data;

namespace KoreanRailwayTrackEditor.Converters
{
    public class AngleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 4 &&
                values[0] is double x1 &&
                values[1] is double y1 &&
                values[2] is double x2 &&
                values[3] is double y2)
            {
                double deltaX = x2 - x1;
                double deltaY = y2 - y1;
                double angle = Math.Atan2(deltaY, deltaX) * 180 / Math.PI;
                // Keep text readable (not upside down)
                if (angle > 90) angle -= 180;
                if (angle < -90) angle += 180;
                return angle;
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
