using System;
using System.Collections.ObjectModel;
using MeetingLauncher.Common.BusinessObjects;

namespace MeetingLauncher.ModernWPF.ViewModels.Design
{
    public class OutlookMeetingsViewModel : ViewModels.OutlookMeetingsViewModel
    {
        public OutlookMeetingsViewModel()
            : base()
        {
            Events = new ObservableCollection<OutlookItem>()
            {
                new OutlookItem()
                {
                    LyncMeeting = new LyncMeeting()
                    {
                        Domain = "magenic.com",
                        MeetingId = "12345",
                        Organizer = "SomeUser",
                        OriginalUri = "example.com"
                    },
                    Start = DateTime.Now.AddHours(1),
                    Subject = "Test Online Meeting"
                },
                new OutlookItem()
                {
                    Start = DateTime.Now.AddHours(2).AddMinutes(30),
                    Subject = "Test Meeting"
                }
            };
        }
    }
}
