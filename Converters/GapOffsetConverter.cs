using System;
using System.Globalization;
using System.Windows.Data;

namespace KoreanRailwayTrackEditor.Converters
{
    public class GapOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 6 && 
                values[0] is double startX && values[1] is double startY &&
                values[2] is double endX && values[3] is double endY &&
                values[4] is double compX && values[5] is double compY)
            {
                // Calculate direction vector (using absolute coordinates)
                double dx = endX - startX;
                double dy = endY - startY;
                double length = Math.Sqrt(dx * dx + dy * dy);
                
                if (length == 0) return startX - compX; // Return relative start
                
                // Normalize and apply gap offset (5 pixels)
                double gapOffset = 5.0;
                double offsetX = (dx / length) * gapOffset;
                double offsetY = (dy / length) * gapOffset;
                
                // Return relative coordinates based on parameter
                string param = parameter as string;
                if (param == "StartX") return (startX + offsetX) - compX;
                if (param == "StartY") return (startY + offsetY) - compY;
                if (param == "EndX") return (endX - offsetX) - compX;
                if (param == "EndY") return (endY - offsetY) - compY;
            }
            
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
