using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MeetingLauncher.Common.BusinessObjects
{
    [Serializable]
    public class LyncMeeting
    {
        private const string CraftyUriStringFormat = "conf:sip:{0}@{1};gruu;opaque=app:conf:focus:id:{2}?required-media=audio";
        private static readonly Regex LinkRegex = new Regex(@"https?://([^\s/]+)/([^\s/]+)/([^\s/]+)/([A-Za-z0-9]+)");

        public LyncMeeting() { }

        public static LyncMeeting ParseLyncMeeting(string uri, string description)
        {
            Match match = LinkRegex.Match(uri);
            if (match.Success)
            {
                return new LyncMeeting
                {
                    OriginalUri = uri,
                    Domain = String.Join(".", match.Groups[1].Value.Split('.').Reverse().Take(2).Reverse()),
                    MeetingId = match.Groups[4].Value.Replace("/", ""),
                    Organizer = match.Groups[3].Value.Replace("/", ""),
                    Description = description
                };                
            }
            else
            {
                return null;
            }
        }

        public static LyncMeeting ParseEmail(string email, string subject)
        {
            var match = LinkRegex.Match(email);
            if (match.Success)
                return ParseLyncMeeting(match.Captures[0].Value, subject);
            return null;
        }

        public string OriginalUri { get; set; }
        public string Domain { get; set; }
        public string Organizer { get; set; }
        public string MeetingId { get; set; }
        public string Description { get; set; }

        public Uri GetCraftyUri()
        {
            return new Uri(String.Format(CraftyUriStringFormat, Organizer, Domain, MeetingId));
        }
    }
}
