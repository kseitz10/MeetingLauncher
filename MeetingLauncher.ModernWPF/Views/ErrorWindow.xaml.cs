using System;
using System.Windows;

namespace MeetingLauncher.ModernWPF.Views
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(Exception e = null)
        {
            InitializeComponent();
            DataContext = this;
            Exception = e;
        }

        public Exception Exception { get; set; }

        public string ExceptionText { get { return Exception.ToString(); } }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
