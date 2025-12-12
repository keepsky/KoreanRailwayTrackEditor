using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using KoreanRailwayTrackEditor.Models;

namespace KoreanRailwayTrackEditor.Converters
{
    public class SignalTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SignalType type)
            {
                string resourceKey = "Signal3Icon"; // Default
                switch (type)
                {
                    case SignalType.Main3:
                        resourceKey = "Signal3Icon";
                        break;
                    case SignalType.Main45:
                        resourceKey = "Signal4Icon";
                        break;
                    case SignalType.Shunt:
                        resourceKey = "SignalShuntIcon";
                        break;
                }
                
                if (System.Windows.Application.Current.Resources.Contains(resourceKey))
                {
                    return System.Windows.Application.Current.Resources[resourceKey];
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
