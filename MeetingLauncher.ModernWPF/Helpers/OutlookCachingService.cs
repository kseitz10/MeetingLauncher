using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using MeetingLauncher.Common.BusinessObjects;
using MeetingLauncher.ModernWPF.Events;

namespace MeetingLauncher.ModernWPF.Helpers
{
    public static class OutlookCachingService
    {
        private static IList<OutlookItem> _cache = new List<OutlookItem>();
        private static readonly object Lock = new object();
        private static readonly DateTime CacheRangeStart = DateTime.Today;
        private static readonly DateTime CacheRangeEnd = DateTime.Today.AddDays(2);
        private static DateTime _cacheLastUpdateTime = DateTime.MinValue;

        public static COMException OutlookException { get; private set; }

        #region Methods
        public static bool UpdateCacheIfNeeded()
        {
            bool needToUpdate = false;
            lock (Lock)
            {
                needToUpdate = _cacheLastUpdateTime.AddMinutes(5) <= DateTime.Now;
            }
            if (needToUpdate)
                UpdateCache();
            return needToUpdate;
        }

        public static void UpdateCache(bool resetException = false)
        {
            if (resetException)
                OutlookException = null;

            if (OutlookException != null)
                return;

            var events = new List<OutlookItem>();

            Microsoft.Office.Interop.Outlook.Application oApp = null;
            Microsoft.Office.Interop.Outlook.NameSpace mapiNamespace = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder calendarFolder = null;
            Microsoft.Office.Interop.Outlook.Items outlookCalendarItems = null;

            try
            {
                oApp = new Microsoft.Office.Interop.Outlook.Application();
                mapiNamespace = oApp.GetNamespace("MAPI");
                calendarFolder = mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);
                outlookCalendarItems = calendarFolder.Items;
                outlookCalendarItems.Sort("[Start]");
                outlookCalendarItems.IncludeRecurrences = true;
                //var filter = String.Format("[Start] >= \"{0}\" and [Start] <= \"{1}\"", DateTime.Today, DateTime.Today.AddDays(1));
                //outlookCalendarItems = outlookCalendarItems.Find(filter);

                foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
                {
                    if (item.Start >= CacheRangeStart)
                    {
                        if (item.Start.Date > CacheRangeEnd)
                            break;

                        var parsed = LyncMeeting.ParseEmail(item.Body ?? string.Empty, item.Subject);
                        events.Add(new OutlookItem()
                        {
                            Start = item.Start,
                            End = item.End,
                            Subject = item.Subject,
                            LyncMeeting = parsed,
                            OutlookAppointment = item
                        });
                    }
                }

                events = events.OrderBy(e => e.Start).ToList();
                lock (Lock)
                {
                    _cache = events;
                    _cacheLastUpdateTime = DateTime.Now;
                }
            }
            catch (COMException e)
            {
                OutlookException = e;
            }
            finally
            {
                Marshal.ReleaseComObject(oApp);
            }
        }

        public static async Task<bool> UpdateCacheIfNeededAsync()
        {
            var wasUpdated = await Task.Run(() => UpdateCacheIfNeeded());
            if(wasUpdated)
                Messenger.Default.Send(new OutlookCacheUpdatedEvent());
            return wasUpdated;
        }

        public static async Task UpdateCacheAsync(bool resetException = false)
        {
            if (resetException)
                OutlookException = null;

            await Task.Run(() => UpdateCache());
            Messenger.Default.Send(new OutlookCacheUpdatedEvent());
        }

        public static IList<OutlookItem> GetCachedMeetings(DateTime? startTime = null, DateTime? endTime = null, bool requireOnlineMeeting = true)
        {
            startTime = startTime ?? DateTime.Today;
            endTime = endTime ?? DateTime.Today.AddDays(1);

            lock (Lock)
            {
                return _cache.Where(e => e.Start >= startTime && e.Start < endTime && (!requireOnlineMeeting || e.LyncMeeting != null)).ToList();
            }
        }
        #endregion

        #region Extension Methods
        public static OutlookItem GetBestMeeting(this IList<OutlookItem> meetings)
        {
            var now = DateTime.Now;

            if (meetings.Any(e => e.LyncMeeting != null))
            {
                var currentMeeting = meetings.FirstOrDefault(e => e.Start <= now && now <= e.End && e.LyncMeeting != null);
                var nextMeeting = meetings.FirstOrDefault(e => e.Start > now && now.AddMinutes(30) >= e.Start && e.LyncMeeting != null);

                if (currentMeeting == null)
                {
                    return nextMeeting;
                }
                else
                {
                    var currentMeetingDuration = currentMeeting.End - currentMeeting.Start;
                    var currentMeetingRemaining = currentMeeting.End - now;

                    // The current meeting is 80% complete, make it secondary if next meeting is available
                    return currentMeetingRemaining.TotalSeconds / currentMeetingDuration.TotalSeconds < 0.20 && nextMeeting != null ? nextMeeting : currentMeeting;
                }
            }
            return null;
        }
        #endregion
    }
}
