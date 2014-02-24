using System;
using System.Windows;
using System.Windows.Data;

namespace MeetingLauncher.ModernWPF.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public bool VisibleIfFalse { get; set; }
        public bool Hidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is bool)
                return (bool)value ^ VisibleIfFalse ? Visibility.Visible : (Hidden ? Visibility.Hidden : Visibility.Collapsed);
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
