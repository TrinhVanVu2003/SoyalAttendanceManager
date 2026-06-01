using System;
using System.IO;

namespace SoyalAttendanceLog
{
    public static class AppLogger
    {
        private static readonly string LogFolder = "logs";
        private static readonly string LogFile = Path.Combine(LogFolder, "app.log");

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Error(string message, Exception ex)
        {
            Write("ERROR", message + " | " + ex.Message);
        }

        private static void Write(string level, string message)
        {
            try
            {
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);

                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                              + " [" + level + "] "
                              + message;

                File.AppendAllText(LogFile, line + Environment.NewLine);
            }
            catch
            {
                // Không để lỗi ghi log làm app bị crash
            }
        }
    }
}