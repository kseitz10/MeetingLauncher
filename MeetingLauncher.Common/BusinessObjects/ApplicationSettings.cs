using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MeetingLauncher.Common.BusinessObjects
{
    [Serializable]
    public class ApplicationSettings
    {
        private static ApplicationSettings _instance = null;

        public bool HideOutlookIntegration { get; set; }
        public bool OutlookIntegration { get; set; }
        public bool ShowTrayIcon { get; set; }
        public bool MinimizeToTray { get; set; }
        public bool DoubleClickConnect { get; set; }

        public List<LyncMeeting> CustomMeetings { get; set; } 

        private ApplicationSettings()
        {
            CustomMeetings = new List<LyncMeeting>();
        }

        public static ApplicationSettings Current
        {
            get { return _instance ?? (_instance = Load()); }
        }

        private static ApplicationSettings Load()
        {
            try
            {
                using (var reader = new StreamReader("meetinglauncher.xml"))
                {
                    var serializer = new XmlSerializer(typeof(ApplicationSettings));
                    var appSettings = (ApplicationSettings)serializer.Deserialize(reader);
                    return appSettings;
                }
            }
            catch (FileNotFoundException)
            {
                return new ApplicationSettings();
            }

        }

        public bool Save()
        {
            try
            {
                using (var writer = new StreamWriter("meetinglauncher.xml"))
                {
                    var serializer = new XmlSerializer(typeof (ApplicationSettings));
                    serializer.Serialize(writer, this);
                    return true;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
