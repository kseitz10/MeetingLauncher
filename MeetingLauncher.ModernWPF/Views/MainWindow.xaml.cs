using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.ViewModels;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.Views
{
    public partial class MainWindow : ModernWindow
    {
        private WindowState _previousWindowState = WindowState.Normal;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel.LaunchMeetingAction = async uri =>
            {
                // You will not be able to comprehend how awesome this is.
                var browser = new WebBrowser();
                browser.Navigate(uri);

                await Task.Delay(500);
                WindowState = WindowState.Minimized;
            };
            Messenger.Default.Register(this, new Action<RequestNavigationEvent>(NavigationRequested));

            if (!ApplicationSettings.Current.Save())
            {
                MessageBox.Show("This application requires a configuration file to be saved in the same directory as the application. You do not have write access to the directory or configuration file. Please correct the issue.", "Settings", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }

            if (!ApplicationSettings.Current.OutlookIntegration && ApplicationSettings.Current.HideOutlookIntegration)
            {
                MenuLinkGroups.First().Links.Remove(MenuLinkGroups.First().Links.First());
                MenuLinkGroups.First().Links.Remove(MenuLinkGroups.First().Links.First());
                ContentSource = new Uri("/Views/CustomMeetingsView.xaml", UriKind.RelativeOrAbsolute);
            }
        }

        private void NavigationRequested(RequestNavigationEvent args)
        {
            ContentSource = new Uri(args.Uri, UriKind.RelativeOrAbsolute);
        }

        public MainWindowViewModel ViewModel
        {
            get
            {
                ((MeetingLauncherViewModelBase)DataContext).Dispatcher = Dispatcher;
                return DataContext as MainWindowViewModel;
            }
        }

        protected override void OnStateChanged(System.EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                if(ApplicationSettings.Current.ShowTrayIcon && ApplicationSettings.Current.MinimizeToTray)
                    ShowInTaskbar = false;
            }
            else
            {
                ShowInTaskbar = true;
                Focus();
                BringIntoView();
            }

            Messenger.Default.Send(new WindowStateChangedEvent()
            {
                WindowState = WindowState,
                PreviousWindowState = _previousWindowState
            });

            _previousWindowState = WindowState;
        }

        private void ContextMenu_ExitApplicationClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContextMenu_ShowApplicationClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            Focus();
            BringIntoView();
        }

        private void ContextMenu_JoinNextMeetingClick(object sender, RoutedEventArgs e)
        {
            ViewModel.JoinNextMeetingCommand.Execute(null);
        }

        private void ContextMenu_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (ApplicationSettings.Current.DoubleClickConnect)
                ContextMenu_JoinNextMeetingClick(sender, e);
            else
                ContextMenu_ShowApplicationClick(sender, e);
        }
    }
}
