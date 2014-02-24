using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.Helpers;
using MeetingLauncher.ModernWPF.Views;

namespace MeetingLauncher.ModernWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "MeetingLauncherInstanceString";

        public List<string> Parameters { get; private set; } 

        public App()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
                DispatcherUnhandledException += (sender, args) =>
                {
                    var window = new ErrorWindow(args.Exception);
                    window.Owner = MainWindow;
                    window.ShowDialog();
                };

            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);

            string path = assemblyName.Name + ".dll";
            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
            {
                path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }

        [STAThread]
        public static void Main(params string[] args)
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Parameters = args.ToList();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow.WindowState = WindowState.Normal;

            Parameters = args.ToList();
            var toJoin = Parameters.FirstOrDefault(p => p.StartsWith("/join:"));
            if (toJoin != null)
            {
                var meeting = LyncMeeting.ParseLyncMeeting(toJoin.Substring(6), string.Empty);
                if(meeting != null)
                    Messenger.Default.Send(new LaunchMeetingEvent()
                    {
                        SipUrl = meeting.GetCraftyUri()
                    });
            }
            
            return true;
        }
    }
}
