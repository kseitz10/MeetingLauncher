using GalaSoft.MvvmLight.Command;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.ViewModels
{
    public class SettingsViewModel : MeetingLauncherViewModelBase
    {
        public SettingsViewModel() : base()
        {

        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(() => ApplicationSettings.Current.Save())); }
        }
    }
}