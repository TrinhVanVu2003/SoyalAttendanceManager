using System.IO;

namespace SoyalAttendanceLog
{
    public class AppConfigService
    {
        private const string ConfigFile = "config.ini";

        public void Save(AppConfig config)
        {
            string[] lines =
            {
                config.Ip,
                config.Port,
                config.DeviceId
            };

            File.WriteAllLines(ConfigFile, lines);
        }

        public AppConfig Load()
        {
            if (!File.Exists(ConfigFile))
            {
                return new AppConfig
                {
                    Ip = "192.168.1.143",
                    Port = "1621",
                    DeviceId = "1"
                };
            }

            string[] lines = File.ReadAllLines(ConfigFile);

            return new AppConfig
            {
                Ip = lines.Length > 0 ? lines[0] : "192.168.1.143",
                Port = lines.Length > 1 ? lines[1] : "1621",
                DeviceId = lines.Length > 2 ? lines[2] : "1"
            };
        }
    }
}