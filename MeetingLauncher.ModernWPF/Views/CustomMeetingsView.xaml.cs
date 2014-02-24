using System.Threading.Tasks;
using System.Windows;
using MeetingLauncher.ModernWPF.ViewModels;
using MeetingLauncher.ModernWPF.Views.Support;

namespace MeetingLauncher.ModernWPF.Views
{
    public partial class CustomMeetingsView : MeetingLauncherViewBase
    {
        public CustomMeetingsView()
        {
            InitializeComponent();
        }

        public new CustomMeetingsViewModel ViewModel
        {
            get { return DataContext as CustomMeetingsViewModel; }
        }

        private async void Expander_OnExpanded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            UrlTextbox.Focus();
        }
    }
}