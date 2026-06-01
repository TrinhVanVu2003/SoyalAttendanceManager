using System;
using System.Net.Sockets;
using System.Text;

namespace SoyalAttendanceLog
{
    public class RawTcpService
    {
        public string SendHexCommand(string ip, int port, string hexCommand)
        {
            byte[] commandBytes = HexStringToBytes(hexCommand);

            using (var client = new TcpClient())
            {
                var result = client.BeginConnect(ip, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                    return "CONNECT_TIMEOUT";

                client.EndConnect(result);

                using (var stream = client.GetStream())
                {
                    stream.Write(commandBytes, 0, commandBytes.Length);

                    var buffer = new byte[4096];

                    var start = DateTime.Now;

                    while ((DateTime.Now - start).TotalSeconds < 3)
                    {
                        if (stream.DataAvailable)
                        {
                            int count = stream.Read(buffer, 0, buffer.Length);
                            var data = new byte[count];
                            Array.Copy(buffer, data, count);

                            string hex = BytesToHex(data);
                            string ascii = Encoding.ASCII.GetString(data);

                            return "HEX: " + hex + Environment.NewLine +
                                   "ASCII: " + ascii;
                        }

                        System.Threading.Thread.Sleep(100);
                    }

                    return "NO_RESPONSE";
                }
            }
        }

        private byte[] HexStringToBytes(string hex)
        {
            hex = hex.Replace(" ", "")
                     .Replace("-", "")
                     .Replace("0x", "")
                     .Replace("0X", "");

            if (hex.Length % 2 != 0)
                throw new ArgumentException("HEX không hợp lệ");

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        private string BytesToHex(byte[] data)
        {
            var sb = new StringBuilder();

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }
    }
}