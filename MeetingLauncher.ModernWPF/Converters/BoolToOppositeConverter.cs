using System;
using System.Windows.Data;

namespace MeetingLauncher.ModernWPF.Converters
{
    public class BoolToOppositeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return !((bool)value);
            throw new NotSupportedException("Invalid use of BoolToOppositeConverter");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
