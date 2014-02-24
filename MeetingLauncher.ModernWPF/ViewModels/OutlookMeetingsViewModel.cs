using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight.Command;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.Helpers;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.ViewModels
{
    public class OutlookMeetingsViewModel : MeetingLauncherViewModelBase
    {
        #region Overrides
        public async override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (ApplicationSettings.Current.OutlookIntegration)
            {
                await OutlookCachingService.UpdateCacheIfNeededAsync();
                Events = new ObservableCollection<OutlookItem>(OutlookCachingService.GetCachedMeetings(requireOnlineMeeting: !ShowAll));
                OutlookException = OutlookCachingService.OutlookException;
                Messenger.Register(this, new Action<OutlookCacheUpdatedEvent>(e => Dispatcher.Invoke(() => OutlookCacheUpdatedEventHandler(e))));
            }
            else
            {
                Events = null;
                OutlookException = new COMException("Outlook integration is not enabled.");
            }

        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Messenger.Unregister(this);
        }
        #endregion

        #region Event handlers
        private void OutlookCacheUpdatedEventHandler(OutlookCacheUpdatedEvent args)
        {
            Events = new ObservableCollection<OutlookItem>(OutlookCachingService.GetCachedMeetings(requireOnlineMeeting: !ShowAll));
            OutlookException = OutlookCachingService.OutlookException;
        }
        #endregion

        #region Properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); JoinMeetingCommand.RaiseCanExecuteChanged(); }
        }

        private bool _showAll;
        public bool ShowAll
        {
            get { return _showAll; }
            set
            {
                _showAll = value;
                OnPropertyChanged();
                Events = new ObservableCollection<OutlookItem>(OutlookCachingService.GetCachedMeetings(requireOnlineMeeting: !ShowAll));
            }
        }

        private ObservableCollection<OutlookItem> _events;
        public ObservableCollection<OutlookItem> Events
        {
            get { return _events; }
            set { _events = value; OnPropertyChanged(); }
        }

        private Exception _outlookException;
        public Exception OutlookException
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
                return _joinMeetingCommand ?? (_joinMeetingCommand = new RelayCommand<OutlookItem>(m => Messenger.Send(new LaunchMeetingEvent()
                {
                    SipUrl = m.LyncMeeting.GetCraftyUri()
                }), m => m != null && m.LyncMeeting != null));
            }
        }

        private RelayCommand<OutlookItem> _showInOutlookCommand;
        public RelayCommand<OutlookItem> ShowInOutlookCommand
        {
            get
            {
                return _showInOutlookCommand ?? (_showInOutlookCommand = new RelayCommand<OutlookItem>(m => m.OutlookAppointment.Display(), m => m != null && m.OutlookAppointment != null));
            }
        }
        #endregion
    }
}