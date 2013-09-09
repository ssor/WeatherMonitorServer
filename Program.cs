using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WeatherMonitorServer
{
    class Program
    {
        static long totalByteCount = 0;
        static byte[] myReadBuffer = new byte[1024];
        static StringBuilder sbuilder = new StringBuilder();
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            client.BeginConnect("172.16.180.10", 9005, new AsyncCallback(connectCallback), client);

            Console.ReadLine();
        }

        private static void connectCallback(IAsyncResult ar)
        {
            Console.WriteLine("成功建立TCP连接...");
            try
            {
                TcpClient tcpclient = ar.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(ar);
                }

                NetworkStream stream = tcpclient.GetStream();

                stream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, readCallback, stream);
                Console.WriteLine("数据传输中...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void readCallback(IAsyncResult ar)
        {
            NetworkStream myNetworkStream = (NetworkStream)ar.AsyncState;
            int numberOfBytesRead;

            numberOfBytesRead = myNetworkStream.EndRead(ar);
            totalByteCount += numberOfBytesRead;
            Console.WriteLine("总共接收数据量(byte)： " + totalByteCount.ToString());
            sbuilder.Append(Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            // message received may be larger than buffer size so loop through until you have it all.
            myNetworkStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length,
                                           new AsyncCallback(readCallback),
                                           myNetworkStream);

            int lastIndex = sbuilder.ToString().LastIndexOf("</MotePacket>");
            if (lastIndex > 0)
            {
                int length = lastIndex + 13;
                string complete = sbuilder.ToString().Substring(0, length);
                sbuilder.Remove(0, length);
                NodeInfoParser.UpdateNodeInfo(complete);
#if DEBUG
                Debug.WriteLine("********************************");
                Debug.WriteLine(complete.Replace("\0", ""));
                Debug.WriteLine("********************************");
#endif
            }
            if (sbuilder.Length > 0)
            {
                Debug.WriteLine(sbuilder.ToString().Replace("\0", ""));
            }
        }
    }
}
