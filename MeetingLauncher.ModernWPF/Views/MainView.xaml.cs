using MeetingLauncher.ModernWPF.ViewModels;
using MeetingLauncher.ModernWPF.ViewModels.Support;
using MeetingLauncher.ModernWPF.Views.Support;

namespace MeetingLauncher.ModernWPF.Views
{
    public partial class MainView : MeetingLauncherViewBase
    {
        public MainView()
        {
            InitializeComponent();
        }

        public new MainViewModel ViewModel
        {
            get
            {
                ((MeetingLauncherViewModelBase) DataContext).Dispatcher = Dispatcher;
                return DataContext as MainViewModel;
            }
        }
    }
}