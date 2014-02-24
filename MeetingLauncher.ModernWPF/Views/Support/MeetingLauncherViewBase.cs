using System.Windows.Controls;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.Views.Support
{
    public abstract class MeetingLauncherViewBase : UserControl, IContent
    {
        #region Virtual Properties
        public virtual MeetingLauncherViewModelBase ViewModel
        {
            get
            {
                ((MeetingLauncherViewModelBase) DataContext).Dispatcher = Dispatcher;
                return DataContext as MeetingLauncherViewModelBase;
            }
            set { DataContext = value; }
        }
        #endregion

        #region Virtual Methods
        public virtual void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            ViewModel.OnFragmentNavigation(e);
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.OnNavigatedFrom(e);
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.OnNavigatedTo(e);
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.OnNavigatingFrom(e);
        }
        #endregion
    }
}
