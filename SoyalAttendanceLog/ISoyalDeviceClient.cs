using System.Collections.Generic;

namespace SoyalAttendanceLog
{
    public interface ISoyalDeviceClient
    {
        bool TestConnection(
            string ip,
            int port);

        List<AttendanceLog> ReadLogs(
            string ip,
            int port,
            string deviceId);
    }
}