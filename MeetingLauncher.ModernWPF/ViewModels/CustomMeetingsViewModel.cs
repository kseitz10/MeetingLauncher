using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using GalaSoft.MvvmLight.Command;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.Helpers;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.ViewModels
{
    public class CustomMeetingsViewModel : MeetingLauncherViewModelBase
    {
        #region Overrides
        public override void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CustomMeetings = new ObservableCollection<LyncMeeting>(ApplicationSettings.CustomMeetings.OrderBy(cm => cm.Description));
            CustomMeetings.CollectionChanged += (sender, args) =>
            {
                ApplicationSettings.CustomMeetings = CustomMeetings.ToList();
                ApplicationSettings.Save();
            };
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); }
        }

        private bool _isAdding;
        public bool IsAdding
        {
            get { return _isAdding; }
            set
            {
                _isAdding = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<LyncMeeting> _customMeetings;
        public ObservableCollection<LyncMeeting> CustomMeetings
        {
            get { return _customMeetings; }
            set
            {
                _customMeetings = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
                AddMeetingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _meetingUrl;
        public string MeetingUrl
        {
            get { return _meetingUrl; }
            set
            {
                _meetingUrl = value;
                OnPropertyChanged();
                AddMeetingCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Commands
        private RelayCommand<LyncMeeting> _joinMeetingCommand;
        public RelayCommand<LyncMeeting> JoinMeetingCommand
        {
            get
            {
                return _joinMeetingCommand ?? (_joinMeetingCommand = new RelayCommand<LyncMeeting>(m => Messenger.Send(new LaunchMeetingEvent()
                {
                    SipUrl = m.GetCraftyUri()
                }), m => m != null));
            }
        }

        private RelayCommand<LyncMeeting> _removeMeetingCommand;
        public RelayCommand<LyncMeeting> RemoveMeetingCommand
        {
            get
            {
                return _removeMeetingCommand ?? (_removeMeetingCommand = new RelayCommand<LyncMeeting>(m => CustomMeetings.Remove(m), m => m != null));
            }
        }

        private RelayCommand<LyncMeeting> _pinMeetingCommand;
        public RelayCommand<LyncMeeting> PinMeetingCommand
        {
            get
            {
                return _pinMeetingCommand ?? (_pinMeetingCommand = new RelayCommand<LyncMeeting>(m =>
                {
                    var jumpList = JumpList.GetJumpList(Application.Current) ?? new JumpList();
                    jumpList.AddMeeting(m, "Custom Meetings");
                    jumpList.Apply();
                }, m => m != null));
            }
        }

        private RelayCommand _addMeetingCommand;
        public RelayCommand AddMeetingCommand
        {
            get
            {
                return _addMeetingCommand ?? (_addMeetingCommand = new RelayCommand(() =>
                {
                    var lyncMeeting = LyncMeeting.ParseLyncMeeting(MeetingUrl, Description);
                    if (lyncMeeting != null)
                    {
                        CustomMeetings.Add(lyncMeeting);
                        MeetingUrl = string.Empty;
                        Description = string.Empty;
                        IsAdding = false;
                    }
                    else
                    {
                        MessageBox.Show("Unable to parse meeting uri.");
                    }    
                }, () => !string.IsNullOrWhiteSpace(MeetingUrl) && !string.IsNullOrWhiteSpace(Description)));
            }
        }
        #endregion
    }
}