using System;
using System.Text;

namespace MeetingLauncher.Common.BusinessObjects
{
    public class OutlookItem : IEquatable<OutlookItem>
    {
        public string Subject { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public LyncMeeting LyncMeeting { get; set; }
        public Microsoft.Office.Interop.Outlook.AppointmentItem OutlookAppointment { get; set; }

        public string FriendlyStart
        {
            get
            {
                var timeTilStart = Start - DateTime.Now;
                var timeFormat = new Func<int, string, string>((i, s) => String.Format("{0} {1}{2}", i, s, (Math.Abs(i) > 1 || i == 0 ? "s" : String.Empty)));

                if (DateTime.Now <= End && DateTime.Now >= Start)
                    return "Now";
                if (DateTime.Today < Start.Date)
                    return "Tomorrow";
                if (DateTime.Now > End)
                    return "Ended";

                var builder = new StringBuilder();
                if (timeTilStart.Hours > 0)
                    builder.Append(timeFormat(timeTilStart.Hours, "hr")).Append(" ");
                if(timeTilStart.Minutes >= 0)
                    builder.Append(timeFormat(timeTilStart.Minutes, "min")).Append(" ");
                return builder.ToString().Trim();
            }
        }

        public bool Equals(OutlookItem other)
        {
            return other != null && other.Start == Start && other.Subject == Subject && (other.LyncMeeting == null || LyncMeeting == null || other.LyncMeeting.OriginalUri == LyncMeeting.OriginalUri);
        }
    }
}
