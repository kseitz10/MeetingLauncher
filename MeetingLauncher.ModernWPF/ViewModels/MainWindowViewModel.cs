using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using GalaSoft.MvvmLight.Command;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;
using MeetingLauncher.ModernWPF.Helpers;
using MeetingLauncher.ModernWPF.ViewModels.Support;

namespace MeetingLauncher.ModernWPF.ViewModels
{
    public class MainWindowViewModel : MeetingLauncherViewModelBase
    {
        public MainWindowViewModel()
        {
            Messenger.Register(this, new Action<LaunchMeetingEvent>(LaunchMeetingEventHandler));
            Messenger.Register(this, new Action<OutlookCacheUpdatedEvent>(e => Dispatcher.Invoke(() => OutlookCacheUpdatedEventHandler(e))));

            var toJoin = ((App) Application.Current).Parameters.FirstOrDefault(p => p.StartsWith("/join:"));
            if (toJoin != null)
            {
                var meeting = LyncMeeting.ParseLyncMeeting(toJoin.Substring(6), string.Empty);
                if (meeting != null)
                    Task.Run(() => Thread.Sleep(500)).ContinueWith(t => Messenger.Send(new LaunchMeetingEvent()
                    {
                        SipUrl = meeting.GetCraftyUri()
                    }), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        #region Event Handlers
        private void LaunchMeetingEventHandler(LaunchMeetingEvent args)
        {
            if (LaunchMeetingAction != null)
                LaunchMeetingAction(args.SipUrl);
        }

        private void OutlookCacheUpdatedEventHandler(OutlookCacheUpdatedEvent args)
        {
            var jumpList = JumpList.GetJumpList(Application.Current) ?? new JumpList();
            JumpList.SetJumpList(Application.Current, jumpList);

            var allTasks = jumpList.GetJumpTasks().ToList();
            //if (!allTasks.Any(t => t.Arguments == "/quickjoin"))
            //    jumpList.Add("Instantly Join Meeting", "Quick Actions", "/quickjoin");

            allTasks.Where(t => t.CustomCategory == "Outlook Calendar").ToList().ForEach(t => jumpList.Remove(t.Arguments));
            OutlookCachingService.GetCachedMeetings()
                .ToList()
                .ForEach(oi => jumpList
                    .Add(String.Format("{0} ({1})", oi.Subject, oi.Start.ToShortTimeString()),
                        "Outlook Calendar",
                        "/join:" + oi.LyncMeeting.OriginalUri));
            jumpList.Apply();
        }
        #endregion

        #region View/ViewModel Communication
        public Action<Uri> LaunchMeetingAction { get; set; }
        #endregion

        #region Commands
        private RelayCommand _joinNextMeetingCommand;
        public RelayCommand JoinNextMeetingCommand
        {
            get
            {
                return _joinNextMeetingCommand ?? (_joinNextMeetingCommand = new RelayCommand(() =>
                {
                    var mtg = OutlookCachingService.GetCachedMeetings().GetBestMeeting();
                    if (mtg != null)
                        Messenger.Send(
                            new LaunchMeetingEvent()
                            {
                                SipUrl = mtg.LyncMeeting.GetCraftyUri()
                            });

                }));
            }
        }
        #endregion
    }
}