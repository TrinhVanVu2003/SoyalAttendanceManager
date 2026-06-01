using System;

namespace SoyalAttendanceLog
{
    public class AttendanceLog
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime EventTime { get; set; }
        public string VerifyType { get; set; }
        public string EventType { get; set; }
        public string Source { get; set; }
        public string LogIndex { get; set; }

        public AttendanceLog()
        {
            DeviceId = "";
            UserId = "";
            UserName = "";
            VerifyType = "";
            EventType = "";
            Source = "";
        }
    }
}