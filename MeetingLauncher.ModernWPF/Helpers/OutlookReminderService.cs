using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using MeetingLauncher.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetingLauncher.Events;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Outlook;

namespace MeetingLauncher.Helpers
{
    public class OutlookReminderService
    {
        private Microsoft.Office.Interop.Outlook.Application oApp = null;
        private static OutlookReminderService _instance;

        public static OutlookReminderService Current
        {
            get { return _instance ?? (_instance = new OutlookReminderService()); }
        }

        private OutlookReminderService()
        {
            try
            {
                oApp = new Microsoft.Office.Interop.Outlook.Application();
                oApp.Reminders.BeforeReminderShow += (ref bool cancel) =>
                {
                    var breakme = "here";
                };

                oApp.Reminder += item =>
                {
                    if (item is AppointmentItem)
                    {
                        var appt = item as AppointmentItem;
                        var breakme = "here";
                    }
                };

                oApp.Reminders.ReminderFire += reminderObject =>
                {
                    var breakme = "here";
                };

                oApp.Reminders.ReminderAdd += reminderObject =>
                {
                    var breakme = "here";
                };
            }
            catch (COMException e)
            {
            }
        }
    }
}
