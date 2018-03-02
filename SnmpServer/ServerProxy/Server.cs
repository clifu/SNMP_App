using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ServerProxy;
using System.IO;
using System.Runtime.Serialization.Json;
using SnmpServer;

namespace ServerProxy
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
            string serverip = GetLocalIPAddress();
            int port = 11000;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.62.154"), 11000);
            Console.WriteLine("Listening on IP " + "192.168.62.154" + " port: " + port);

            while (true)
            {
                var receive = udpServer.Receive(ref endPoint);
                string time = null;
                time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(time + " Received value is: " + Encoding.ASCII.GetString(receive));
                string sentence = null;
                getObject get = ReadToObject(receive);
                string ip_Address = get.Ip;

                if(get.DeviceName=="Daniel")
                {
                    if (get.ElementName == "sysDescr")
                    {
                        sentence = ".1.3.6.1.2.1.1.1.0";
                    }
                    else if (get.ElementName == "sysObjectID")
                    {
                        sentence = ".1.3.6.1.2.1.1.2.0";
                    }
                    else if (get.ElementName == "sysUpTime")
                    {
                        sentence = ".1.3.6.1.2.1.1.3.0";
                    }
                    else if (get.ElementName == "icmpInMsgs")
                    {
                        sentence = ".1.3.6.1.2.1.5.1.0";
                    }
                    else if (get.ElementName == "sysName")
                    {
                        sentence = ".1.3.6.1.2.1.1.5.0";
                    }
                    else if (get.ElementName == "icmpInDestUnreachs")
                    {
                        sentence = ".1.3.6.1.2.1.5.3.0";
                    }
                    else if (get.ElementName == "sysServices")
                    {
                        sentence = ".1.3.6.1.2.1.1.7.0";
                    }
                    string value = snmp.get(sentence)[2];
                    string oid = snmp.get(sentence)[0];
                    string type = snmp.get(sentence)[1];

                    time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                Console.ForegroundColor = ConsoleColor.Red;



                //string receivedMessage = Encoding.ASCII.GetString(receive);
                //string ipAddress = receivedMessage.Split('#')[1];

                var client = new UdpClient(get.Ip, 11001);

                byte[] msg = WriteFromObject(new SnmpTypeObject(oid, value, type));
                Console.WriteLine(time + " Sending JSON object :" + Encoding.ASCII.GetString(msg));
                int size = msg.Length;
                client.Send(msg, size);
                }
                else
                {
                    var clientProxy = new UdpClient("192.168.60.84", 11000);
                    clientProxy.Send(receive, receive.Length);

                    
                    IPEndPoint endPoint2 = new IPEndPoint(IPAddress.Parse(serverip), 11000);
                   receive= udpServer.Receive(ref endPoint);
                    time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(time + " Sending JSON object :" + Encoding.ASCII.GetString(receive));
                    var client2 = new UdpClient(ip_Address, 11001);
                    client2.Send(receive, receive.Length);

                }

            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public byte[] WriteFromObject(SnmpTypeObject obj)
        {
            //Create User object.  
            SnmpTypeObject objectSnmp = obj;

            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SnmpTypeObject));
            ser.WriteObject(ms, objectSnmp);
            byte[] json = ms.ToArray();
            ms.Close();
            return json;
        }

        public getObject ReadToObject(byte[] json)
        {
            getObject deserializedUser = new getObject();
            MemoryStream ms = new MemoryStream(json);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
            deserializedUser = ser.ReadObject(ms) as getObject;
            ms.Close();
            return deserializedUser;
        }

        private static void StartListening()
        {
            string serverip = GetLocalIPAddress();
            int port = 11000;
            Console.WriteLine("Listening on IP " + serverip + " port: " + port);

            HttpListener listener = new HttpListener();

            SetPrefixes(listener);

            if (listener.Prefixes.Count > 0)
            {
                listener.Start();

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;

                    String url = request.RawUrl;
                    String[] queryStringArray = url.Split('/');



                    HttpListenerResponse response = context.Response;

                    byte[] buffer = null;

                    if (queryStringArray[1] == "1")
                    {
                        buffer = System.Text.Encoding.UTF8.GetBytes("I received myForm");
                    }



                    if (buffer != null)
                    {
                        response.AddHeader("Cache-Control", "no-cache");
                        response.AddHeader("Access-Control-Allow-Origin", "*");

                        response.ContentLength64 = buffer.Length;
                        Stream outputStream = response.OutputStream;
                        outputStream.Write(buffer, 0, buffer.Length);
                        outputStream.Close();
                    }
                }
            }
        }

        private static void SetPrefixes(HttpListener listener)
        {
            string ipAddres = GetLocalIPAddress();
            String[] prefixes = new String[] { "http://" + ipAddres + ":11000/" };

            int i = 0;

            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
                i++;
            }
        }

    }

    public class getObject
    {
        public string DeviceName;
        public string ElementName;
        public string Ip;

        public getObject(string device, string element, string ip)
        {
            this.DeviceName = device;
            this.ElementName = element;
            this.Ip = ip;
        }
        public getObject()
        {

        }
    }
}
