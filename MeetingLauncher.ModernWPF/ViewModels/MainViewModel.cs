using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight.Command;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.Helpers;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.ViewModels
{
    public class MainViewModel : MeetingLauncherViewModelBase
    {
        private readonly Timer _refreshTimer;

        public MainViewModel() : base()
        {
            _refreshTimer = new Timer {AutoReset = true, Interval = 5000};
            _refreshTimer.Elapsed += (sender, args) => Dispatcher.Invoke(() => RefreshMeetingsAsync());
        }

        #region Overrides
        public async override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            Messenger.Register(this, new Action<OutlookCacheUpdatedEvent>(e => Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                PrimaryMeeting = OutlookCachingService.GetCachedMeetings().GetBestMeeting();
                OutlookException = OutlookCachingService.OutlookException;
                IsBusy = false;
            })));

            await Task.Delay(100);
            await RefreshMeetingsAsync();
            _refreshTimer.Start();
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Messenger.Unregister(this);
            _refreshTimer.Stop();
        }
        #endregion

        #region Methods
        public async Task RefreshMeetingsAsync(bool forceRefresh = false)
        {
            IsBusy = true;
            if (ApplicationSettings.Current.OutlookIntegration)
            {
                if (forceRefresh)
                    await OutlookCachingService.UpdateCacheAsync(true);
                else
                    await OutlookCachingService.UpdateCacheIfNeededAsync();
                PrimaryMeeting = OutlookCachingService.GetCachedMeetings().GetBestMeeting();
                OutlookException = OutlookCachingService.OutlookException;
            }
            else
            {
                PrimaryMeeting = null;
                OutlookException = null;
            }
            IsBusy = false;
        }
        #endregion

        #region Private Methods
        private void RaiseCanExecuteChanged()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                JoinMeetingCommand.RaiseCanExecuteChanged();
                RefreshCommand.RaiseCanExecuteChanged();
                ShowMoreCommand.RaiseCanExecuteChanged();
            }));
        }
        #endregion

        #region Properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        private OutlookItem _primaryMeeting;
        public OutlookItem PrimaryMeeting
        {
            get { return _primaryMeeting; }
            set
            {
                _primaryMeeting = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        private OutlookItem _lastMeeting;
        public OutlookItem LastMeeting
        {
            get { return _lastMeeting; }
            set
            {
                _lastMeeting = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        private COMException _outlookException;
        public COMException OutlookException
        {
            get { return _outlookException; }
            set
            {
                _outlookException = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        private RelayCommand<OutlookItem> _joinMeetingCommand;
        public RelayCommand<OutlookItem> JoinMeetingCommand
        {
            get
            {
                return _joinMeetingCommand ?? (_joinMeetingCommand = new RelayCommand<OutlookItem>(m =>
                {
                    Messenger.Send(new LaunchMeetingEvent()
                    {
                        SipUrl = m.LyncMeeting.GetCraftyUri()
                    });
                    LastMeeting = m;
                }, m => !IsBusy));
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new RelayCommand(async () =>
                {
                    OutlookException = null;
                    await RefreshMeetingsAsync(true);
                    _refreshTimer.Stop();
                    _refreshTimer.Start();
                }, () => !IsBusy));
            }
        }

        private RelayCommand _showMoreCommand;
        public RelayCommand ShowMoreCommand
        {
            get
            {
                return _showMoreCommand ?? (_showMoreCommand = new RelayCommand(() => NavigateTo("/Views/OutlookMeetingsView.xaml"), () => !IsBusy));
            }
        }
        #endregion
    }
}