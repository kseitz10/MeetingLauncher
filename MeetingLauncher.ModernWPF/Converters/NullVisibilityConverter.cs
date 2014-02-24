using System;
using System.Windows;
using System.Windows.Data;

namespace MeetingLauncher.ModernWPF.Converters
{
    public class NullVisibilityConverter : IValueConverter
    {
        public bool VisibleIfNull { get; set; }
        public bool Hidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ^ VisibleIfNull ? Visibility.Visible : ( Hidden ? Visibility.Hidden : Visibility.Collapsed );
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
