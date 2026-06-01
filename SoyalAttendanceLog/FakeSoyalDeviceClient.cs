using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SoyalAttendanceLog
{
    public class FakeSoyalDeviceClient : ISoyalDeviceClient
    {
        private DateTime _lastLogTime = DateTime.MinValue;
        private int _counter = 1000;

        public bool TestConnection(string ip, int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                    if (!success)
                        return false;

                    client.EndConnect(result);
                    return client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<AttendanceLog> ReadLogs(string ip, int port, string deviceId)
        {
            var logs = new List<AttendanceLog>();

            if ((DateTime.Now - _lastLogTime).TotalSeconds >= 5)
            {
                _lastLogTime = DateTime.Now;
                _counter++;

                logs.Add(new AttendanceLog
                {
                    DeviceId = deviceId,
                    UserId = _counter.ToString(),
                    UserName = "Nhan vien " + _counter,
                    EventTime = DateTime.Now,
                    VerifyType = "Fingerprint",
                    EventType = "CheckIn",
                    Source = "Fake SOYAL"
                });
            }

            return logs;
        }
        public List<AttendanceLog> GenerateLogs(
    string deviceId,
    int quantity)
        {
            var logs = new List<AttendanceLog>();

            for (int i = 0; i < quantity; i++)
            {
                _counter++;

                logs.Add(new AttendanceLog
                {
                    DeviceId = deviceId,
                    UserId = _counter.ToString(),
                    UserName = "Nhan vien " + _counter,
                    EventTime = DateTime.Now.AddSeconds(-i),
                    VerifyType = "Fingerprint",
                    EventType = (i % 2 == 0) ? "CheckIn" : "CheckOut",
                    Source = "Generated"
                });
            }

            return logs;
        }
    }
}