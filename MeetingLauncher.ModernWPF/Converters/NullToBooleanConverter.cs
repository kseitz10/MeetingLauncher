using System;
using System.Windows.Data;

namespace MeetingLauncher.ModernWPF.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value == null && Reverse) || (!Reverse && value != null) ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
