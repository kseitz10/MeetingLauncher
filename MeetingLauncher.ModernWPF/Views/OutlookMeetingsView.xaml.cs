using MeetingLauncher.ModernWPF.ViewModels;
using MeetingLauncher.ModernWPF.Views.Support;

namespace MeetingLauncher.ModernWPF.Views
{
    public partial class OutlookMeetingsView : MeetingLauncherViewBase
    {
        public OutlookMeetingsView()
        {
            InitializeComponent();
        }

        public new OutlookMeetingsViewModel ViewModel
        {
            get { return DataContext as OutlookMeetingsViewModel; }
        }
    }
}