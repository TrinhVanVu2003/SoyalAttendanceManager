using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SoyalAttendanceLog
{
    public class SoyalWebClient
    {
        public List<AttendanceLog> GetEventLogs(string ip, string deviceId)
        {
            string url = "http://" + ip + "/EventLog.cgi";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["Authorization"] = "Basic VVRtaW46";

            string postData = "eStart=9999&btnLoad=Go+to";
            byte[] data = Encoding.ASCII.GetBytes(postData);

            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string html;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream, Encoding.GetEncoding("big5")))
            {
                html = reader.ReadToEnd();
            }

            return ParseLogs(html, deviceId);
        }

        private List<AttendanceLog> ParseLogs(string html, string deviceId)
        {
            var logs = new List<AttendanceLog>();

            var regex = new Regex(
                @"<tr><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td><td>(.*?)</td>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var matches = regex.Matches(html);

            foreach (Match match in matches)
            {
                string index = Clean(match.Groups[1].Value);
                string date = Clean(match.Groups[2].Value);
                string time = Clean(match.Groups[3].Value);
                string address = Clean(match.Groups[4].Value);
                string alias = Clean(match.Groups[5].Value);
                string detail = Clean(match.Groups[6].Value);
                string cardUid = Clean(match.Groups[7].Value);
                string door = Clean(match.Groups[8].Value);

                if (index == "Index")
                    continue;

                DateTime eventTime = ParseSoyalDateTime(date, time);

                logs.Add(new AttendanceLog
                {
                    LogIndex = index,
                    DeviceId = deviceId,
                    UserId = address,
                    UserName = alias == "null" ? "" : alias,
                    EventTime = eventTime,
                    VerifyType = detail,
                    EventType = detail.Contains("FingerPrint") ? "Fingerprint" : detail,
                    Source = "SOYAL Web EventLog.cgi"
                });
            }

            return logs;
        }

        private string Clean(string value)
        {
            return WebUtility.HtmlDecode(value)
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "")
                .Trim();
        }

        private DateTime ParseSoyalDateTime(string date, string time)
        {
            // SOYAL format: 26'06/01 = 2026/06/01
            string[] parts = date.Replace("'", "/").Split('/');

            int year = 2000 + int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            var timeParts = time.Split(':');

            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);
            int second = int.Parse(timeParts[2]);

            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}