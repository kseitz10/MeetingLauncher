using System.Windows;

namespace MeetingLauncher.ModernWPF.Events
{
    public class WindowStateChangedEvent
    {
        public WindowState WindowState { get; set; }
        public WindowState PreviousWindowState { get; set; }
    }
}
