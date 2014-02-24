using System.Collections.Generic;
using System.Linq;
using System.Windows.Shell;
using MeetingLauncher.Common.BusinessObjects;

namespace MeetingLauncher.ModernWPF.Helpers
{
    public static class JumpListHelper
    {
        public static IEnumerable<JumpTask> GetJumpTasks(this JumpList jumpList)
        {
            return jumpList.JumpItems.OfType<JumpTask>();
        }

        public static void AddMeeting(this JumpList jumpList, LyncMeeting meeting, string category = "Meetings")
        {
            jumpList.JumpItems.Add(new JumpTask()
            {
                ApplicationPath = System.Reflection.Assembly.GetExecutingAssembly()
                                        .Location,
                Arguments = "/join:"+ meeting.OriginalUri,
                CustomCategory = category,
                Title = meeting.Description,
                WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly()
                                                                            .Location)
            });
        }

        public static void Add(this JumpList jumpList, string title, string category, string arguments)
        {
            if (!jumpList.JumpItems.Any(ji => ji is JumpTask && ((JumpTask) ji).Arguments == arguments))
                jumpList.JumpItems.Add(new JumpTask()
                {
                    ApplicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location,
                    Arguments = arguments,
                    CustomCategory = category,
                    Title = title,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                });
        }

        public static void Remove(this JumpList jumpList, string arguments)
        {
            jumpList.JumpItems.RemoveAll(j => j is JumpTask && ((JumpTask)j).Arguments == arguments);
        }

        public static void RemoveMeeting(this JumpList jumpList, LyncMeeting meeting)
        {
            jumpList.JumpItems.RemoveAll(j => j is JumpTask && ((JumpTask)j).Arguments == meeting.OriginalUri);
        }
    }
}
