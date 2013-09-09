using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Fleck;
using Newtonsoft.Json;

namespace WeatherMonitorServer
{
    class Program
    {

        #region Members
        static string strNodesDes = string.Empty;
        static Timer timerActOnInputData = new Timer();
        static long totalByteCount = 1000000;
        static byte[] myReadBuffer = new byte[1024];
        static StringBuilder sbuilderInputData = new StringBuilder();

        static List<IWebSocketConnection> ClientList = new List<IWebSocketConnection>();
        #endregion


        static void Main(string[] args)
        {
            updateConfig();


            TcpClient client = new TcpClient();

            client.BeginConnect("172.16.180.10", 9005, new AsyncCallback(connectCallback), client);

            NodeInfoParser.NotifyNodeChange = ReportChangedNode;

            timerActOnInputData.Interval = 300;
            timerActOnInputData.Elapsed += timerActOnInputData_Elapsed;
            timerActOnInputData.Start();

            StartWebSocketServer(9905);


            string line;
        READ_LOOP: line = Console.ReadLine();
            if (line == "update")
            {
                updateConfig();
            }

            goto READ_LOOP;
        }
        static void updateConfig()
        {
            List<NodeDes> NodeDesList = NodeDes.importData();
            strNodesDes = JsonConvert.SerializeObject(NodeDesList);
            Console.WriteLine("节点信息已更新：");
            Console.WriteLine(strNodesDes);
        }
        static void timerActOnInputData_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (sbuilderInputData.Length > 0)
            {
                string temp = sbuilderInputData.ToString();
                int lastIndex = temp.LastIndexOf("</MotePacket>");
                if (lastIndex > 0)
                {
                    int length = lastIndex + 13;
                    string complete = temp.Substring(0, length);
                    sbuilderInputData.Remove(0, length);
                    NodeInfoParser.UpdateNodeInfo(complete);
#if DEBUG
                    Debug.WriteLine("********************************");
                    Debug.WriteLine(complete.Replace("\0", ""));
                    Debug.WriteLine("********************************");
#endif
                }

                Debug.WriteLine(sbuilderInputData.ToString().Replace("\0", ""));
            }
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
            sbuilderInputData.Append(Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            // message received may be larger than buffer size so loop through until you have it all.
            myNetworkStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length,
                                           new AsyncCallback(readCallback),
                                           myNetworkStream);



        }
        #region WebSocket Server
        static void StartWebSocketServer(int _websocketPort)
        {
            string url = "ws://localhost:" + _websocketPort.ToString();
            WebSocketServer server = new WebSocketServer(url);
            server.Start(socket =>
            {
                string originurl = socket.ConnectionInfo.Host + socket.ConnectionInfo.Path;
                socket.OnOpen = () =>
                {
                    Console.WriteLine(originurl + " connected");

                    if (socket.ConnectionInfo.Path == "/Client")
                    {
                        addClient(socket, ClientList);
                        Debug.WriteLine("Client ++  => " + ClientList.Count.ToString());
                    }
                    command c = new command("nodeDes", strNodesDes);
                    socket.Send(JsonConvert.SerializeObject(c) as string);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine(originurl + " closed");
                    removeClient(socket, ClientList);

                };
                socket.OnMessage = message =>
                {
                    Debug.WriteLine("OnMessage => " + message);

                    //List<TagInfo> tags = TagPool.GetAllExistsTags();
                    //string json = JsonConvert.SerializeObject(tags);
                    //socket.Send(json);
                    if (message == "nodeDes")
                    {
                        command c = new command("nodeDes", strNodesDes);
                        socket.Send(JsonConvert.SerializeObject(c) as string);
                    }
                    if (message == "nodes")
                    {
                        command c = new command("nodes", NodeInfoParser.GetNodesJson());
                        socket.Send(JsonConvert.SerializeObject(c) as string);
                        //socket.Send(NodeInfoParser.GetNodesJson());
                    }

                };
                socket.OnError = (error) =>
                {
                    Debug.WriteLine("OnError => " + error.Data);
                    removeClient(socket, ClientList);

                };
            });
        }


        static void removeClient(IWebSocketConnection client, List<IWebSocketConnection> list)
        {
            IWebSocketConnection c = list.Find((_client) =>
            {
                return _client.ConnectionInfo.Id == client.ConnectionInfo.Id;
            });
            if (c != null)
            {
                list.Remove(client);
                //Debug.WriteLine("Client --  => " + list.Count.ToString());
            }


        }


        static void addClient(IWebSocketConnection client, List<IWebSocketConnection> list)
        {
            IWebSocketConnection c = list.Find((_client) =>
            {
                return _client.ConnectionInfo.Origin == client.ConnectionInfo.Origin;
            });
            if (c == null)
            {
                list.Add(client);
                //Debug.WriteLine("Client ++  => " + list.Count.ToString());
            }
        }
        static void ReportChangedNode(Node _node)
        {
            List<Node> list = new List<Node> { _node };
            string json = JsonConvert.SerializeObject(list);
            Debug.WriteLine("Node {0} Changed!", _node.nodeId);

            command c = new command("nodes", json);
            string sToSend = (JsonConvert.SerializeObject(c) as string);
            List<IWebSocketConnection> clientList = new List<IWebSocketConnection>(ClientList);
            foreach (IWebSocketConnection socket in clientList)
            {

                socket.Send(sToSend);
            }
        }
        #endregion

    }

    public class command
    {
        public string name = string.Empty;
        public string content = string.Empty;
        public command(string _name, string _content)
        {
            this.name = _name;
            this.content = _content;
        }
    }

}
