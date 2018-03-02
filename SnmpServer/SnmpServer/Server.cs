using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using SnmpServer;

namespace SnmpServer
{
    public class Server
    {
        Snmp snmp;

        public Server()
        {
            snmp = new Snmp();
            waitForConnection();
        }

        public void waitForConnection()
        {
            UdpClient udpServer = new UdpClient(11000);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"),11000);
            Console.WriteLine("Listening...");

            while (true)
            {
                var receive = udpServer.Receive(ref endPoint);
                string time = null;
                time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(time + " Received value is: " + Encoding.ASCII.GetString(receive));

                string value = snmp.get(Encoding.ASCII.GetString(receive))[2];
                if (value.Equals(""))
                {
                    value = "Empty";
                }
                time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(time + " Sending value: " + value);

                var client = new UdpClient("192.168.0.101", 11001);

                byte[] msg = Encoding.ASCII.GetBytes(value);
                int size = Encoding.ASCII.GetByteCount(value);
                client.Send(msg, size);
            }
        }
    }
}
