using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace MeetingLauncher.ModernWPF.ViewModels.Support
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            Register<MainWindowViewModel>();
            Register<SettingsViewModel>();
            Register<MainViewModel>();
            Register<CustomMeetingsViewModel>();
            Register<OutlookMeetingsViewModel, Design.OutlookMeetingsViewModel>();
        }



        public MainWindowViewModel MainWindow
        {
            get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); }
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public OutlookMeetingsViewModel OutlookMeetings
        {
            get { return ServiceLocator.Current.GetInstance<OutlookMeetingsViewModel>(); }
        }

        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public CustomMeetingsViewModel CustomMeetings
        {
            get { return ServiceLocator.Current.GetInstance<CustomMeetingsViewModel>(); }
        }



        private void Register<TInterface, TDesignType, TNormalType>()
            where TInterface : class
            where TDesignType : class
            where TNormalType : class
        {
            if (SimpleIoc.Default.IsRegistered<TInterface>())
                return;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<TInterface, TDesignType>();
            }
            else
            {
                SimpleIoc.Default.Register<TInterface, TNormalType>();
            }
        }

        private void Register<TViewModel, TViewModelDesign>()
            where TViewModel : class
            where TViewModelDesign : class
        {
            if (SimpleIoc.Default.IsRegistered<TViewModel>())
                return;

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<TViewModel, TViewModelDesign>();
            }
            else
            {
                SimpleIoc.Default.Register<TViewModel>();
            }
        }

        private void Register<TViewModel>() where TViewModel : class
        {
            Register<TViewModel, TViewModel>();
        }
    }
}