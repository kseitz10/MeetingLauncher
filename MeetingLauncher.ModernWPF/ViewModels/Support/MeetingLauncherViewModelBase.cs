using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;

namespace MeetingLauncher.ModernWPF.ViewModels.Support
{
    public abstract class MeetingLauncherViewModelBase : ViewModelBase, INotifyPropertyChanged
    {
        public IMessenger Messenger
        {
            get { return GalaSoft.MvvmLight.Messaging.Messenger.Default; }
        }

        private Dispatcher _dispatcher;
        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set
            {
                _dispatcher = value;
                OnPropertyChanged();
            }
        }

        public ApplicationSettings ApplicationSettings
        {
            get { return ApplicationSettings.Current; }
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            RaisePropertyChanged(propertyName);
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged<T>(propertyExpression);
        }

        protected void SetValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (oldValue == null || !oldValue.Equals(newValue))
            {
                oldValue = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        protected void NavigateTo(string path)
        {
            Messenger.Send(new RequestNavigationEvent()
            {
                Uri = path
            });
        }

        #region Virtual methods for navigation
        public virtual void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }
        #endregion
    }
}
