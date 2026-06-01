using System;
using System.Collections.Generic;

namespace SoyalAttendanceLog
{
    public class SoyalDeviceClient : ISoyalDeviceClient
    {
        public bool TestConnection(string ip, int port)
        {
            throw new NotImplementedException();
        }

        public List<AttendanceLog> ReadLogs(
            string ip,
            int port,
            string deviceId)
        {
            throw new NotImplementedException();
        }
    }
}