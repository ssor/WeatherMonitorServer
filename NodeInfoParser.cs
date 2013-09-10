
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace WeatherMonitorServer
{
    // node 6  1. temperature   2. humidity
    public class Node
    {
        //节点ID
        public string nodeId = string.Empty;
        public string temperature = string.Empty;
        public string humidity = string.Empty;
        public string soilMoisture = string.Empty;
        public string soilTemperature = string.Empty;
        public string Solar = string.Empty;
        public string WindMax = string.Empty;
        public string WindAvg = string.Empty;
        public string WindDirAvg = string.Empty;
        public string RainRate = string.Empty;
        public string BP = string.Empty;
        //public string RainTotal = string.Empty;
        public string bUpdated = "false";
        //public string dewPoint = string.Empty;
        public Node() { }
        public Node(string _id)
        {
            this.nodeId = _id;
        }
        public Node UpdateNewNode(Node _node)
        {

            Type type = Type.GetType("WeatherMonitorServer.Node");
            FieldInfo[] fields = type.GetFields();
            fields.All(_field =>
            {
                if (_field.Name == "nodeId" || _field.Name == "bUpdated")
                {
                    return true;
                }
                string newValue = (string)_field.GetValue(_node);
                string currentValue = (string)_field.GetValue(this);
                if (newValue != string.Empty)
                {
                    if (newValue != currentValue)
                    {
                        Debug.WriteLine(string.Format("更新项：{0} {1} =>  {2}", _field.Name, currentValue, newValue));
                        _node.bUpdated = "true";
                    }
                }
                if (newValue == string.Empty && currentValue != string.Empty)
                {
                    _field.SetValue(_node, currentValue);
                }
                return true;
            });

            return _node;
        }
        public string formatedString()
        {
            return string.Format("id => {0} temp:{1} humi:{2} \r\n soil:{3},{4} \r\n Weather:{5}(Solar),{6}(WindMax),{7}(WindAvg),{8}(WindDirAvg),{9}(RainRate),{10}(BP)",
                nodeId, temperature, humidity, soilMoisture, soilTemperature, Solar, WindMax, WindAvg, WindDirAvg, RainRate, BP);
        }
    }
    public class NodeInfoParser
    {
        public static Action<Node> NotifyNodeChange = null;
        //public static List<Node> NodeList = null;
        public static Dictionary<string, Node> NodeDic = null;
        public static string GetNodesJson()
        {
            return JsonConvert.SerializeObject(NodeDic.Values.ToList<Node>());
        }
        public static void UpdateNodeDic(List<NodeDes> list)
        {
            if (NodeDic == null)
            {
                NodeDic = new Dictionary<string, Node>();
            }
            foreach (NodeDes nd in list)
            {
                if (!NodeDic.ContainsKey(nd.nodeID))
                {
                    NodeDic.Add(nd.nodeID, new Node(nd.nodeID));
                }
            }
        }
        public static List<Node> UpdateNodeInfo(string _xml)
        {
            if (NodeDic == null)
            {
                NodeDic = new Dictionary<string, Node>();
                //NodeList = new List<Node> { new Node("2"), new Node("3"), new Node("4"), new Node("5"), new Node("6") };
                //NodeDic.Add("2", new Node("2"));
                //NodeDic.Add("3", new Node("3"));
                //NodeDic.Add("4", new Node("4"));
                //NodeDic.Add("5", new Node("5"));
                //NodeDic.Add("6", new Node("6"));
                //NodeDic.Add("7", new Node("7"));
            }

            List<Node> nodeList = ParseXmlToNodeList(_xml);
            if (nodeList.Count > 0)
            {
                nodeList.All(_node =>
                {
                    if (NodeDic.ContainsKey(_node.nodeId))
                    {
                        Node n = NodeDic[_node.nodeId].UpdateNewNode(_node);
                        if (n != null)
                        {
                            NodeDic[n.nodeId] = n;
                            //notify changed node
                            if (NotifyNodeChange != null)
                            {
                                NotifyNodeChange(n);
                            }
                        }
#if DEBUG
                        Debug.WriteLine("---------------------------------------------------");
                        if (n != null)
                        {
                            Debug.WriteLine(n.formatedString());
                        }
                        Debug.WriteLine("---------------------------------------------------");
#endif
                    }
                    return true;
                });
            }
            return NodeDic.Values.ToList<Node>();
        }

        public static List<Node> ParseXmlToNodeList(string _xml)
        {
            _xml = _xml.Replace("\r\n", "");
            List<Node> nodeList = new List<Node>();
            string regexExpress = "\\<\\?xml version\\=\\\"1.0\" \\?\\>\\<MotePacket\\>[\\<\\w\\> ./]+\\</MotePacket\\>";
            MatchCollection mc = Regex.Matches(_xml, regexExpress);
            for (int i = 0; i < mc.Count; i++)
            {
                Match m = mc[i];
                string regVaue = m.Value;
                Func<string, Node> parser = GetParseFunc(regVaue);
                if (null != parser)
                {
                    Node n = parser(regVaue);
#if DEBUG
                    Debug.WriteLine(regVaue);
                    //Debug.WriteLine("---------------------------------------------------");
                    //if (n != null)
                    //{
                    //    Debug.WriteLine(n.formatedString());
                    //}
#endif
                    nodeList.Add(n);
                }
            }
            return nodeList;
        }

        private static Func<string, Node> GetParseFunc(string regVaue)
        {
            var weatherFlag = "Weather Sensor";
            if (regVaue.IndexOf(weatherFlag) >= 0)
            {
                return ParseNodeXml_ET22WeatherSensor;
            }
            var soilTempFlag = "Soil Temperature Sensor";
            if (regVaue.IndexOf(soilTempFlag) >= 0)
            {
                return ParseNodeXml_SoilTemperatureSensor;
            }

            var soilMoistureFlag = "Soil Moisture Sensor";
            if (regVaue.IndexOf(soilMoistureFlag) >= 0)
            {
                return ParseNodeXml_eS1100SoilMoistureSensorV1;
            }
            var ambientSensorFlag = "Ambient Temperature and Humidity Sensor";
            if (regVaue.IndexOf(ambientSensorFlag) >= 0)
            {
                return ParseNodeXml_eS1201AmbientTemperatureAndHumiditySensor;
            }
            return null;
        }
        public static Node ParseNodeXml_ET22WeatherSensor(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("nodeId", xd, newNode, string.Empty, nodeIdFormatter);
            ParseNodeValue("Solar", xd, newNode);
            ParseNodeValue("WindMax", xd, newNode);
            ParseNodeValue("WindAvg", xd, newNode);
            ParseNodeValue("WindDirAvg", xd, newNode);
            ParseNodeValue("RainRate", xd, newNode);
            ParseNodeValue("BP", xd, newNode);
            ParseNodeValue("RainTotal", xd, newNode);
            ParseNodeValue("Temp", xd, newNode, "temperature");
            ParseNodeValue("Humidity", xd, newNode, "humidity");
            return newNode;
        }

        public static Node ParseNodeXml_SoilTemperatureSensor(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("nodeId", xd, newNode, string.Empty, nodeIdFormatter);
            ParseNodeValue("temp", xd, newNode, "soilTemperature");
            return newNode;
        }

        public static Node ParseNodeXml_eS1100SoilMoistureSensorV1(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("nodeId", xd, newNode, string.Empty, nodeIdFormatter);
            ParseNodeValue("soilMoisture", xd, newNode);
            return newNode;
        }

        public static Node ParseNodeXml_eS1201AmbientTemperatureAndHumiditySensor(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("nodeId", xd, newNode, string.Empty, nodeIdFormatter);
            ParseNodeValue("temperature", xd, newNode);
            ParseNodeValue("humidity", xd, newNode);
            return newNode;
        }

        #region Helper
        public static Node SetFieldValueDynamic(Node _node, string _fieldName, string _value)
        {
            Type type = Type.GetType("WeatherMonitorServer.Node");
            FieldInfo fieldInfo = type.GetField(_fieldName);
            fieldInfo.SetValue(_node, _value);
            return _node;
        }
        static string nodeIdFormatter(string _id)
        {
            if (_id.IndexOf(".") > 0)
            {
                return _id.Substring(0, _id.IndexOf("."));
            }
            else
                return _id;
        }
        static Node ParseNodeValue(string property, XmlDocument _xd, Node _node, string _nodeFieldName = "", Func<string, string> valueFormatter = null)
        {
            if (_node == null)
            {
                _node = new Node();
            }
            if (_nodeFieldName == "" || _nodeFieldName == null) _nodeFieldName = property;
            //XmlNode nodeID = _xd.SelectSingleNode("/MotePacket/NodeId");
            //string id = nodeID.InnerText;
            //_node.nodeId = id.Substring(0, id.IndexOf("."));
            XmlNodeList list = _xd.SelectNodes("/MotePacket/ParsedDataElement");
            foreach (XmlNode xn in list)
            {
                XmlNode nodeName = xn.SelectSingleNode("Name");
                if (nodeName.InnerText == property)
                {
                    XmlNode nodeValue = xn.SelectSingleNode("ConvertedValue");
                    string value = nodeValue.InnerText;
                    if (valueFormatter != null) value = valueFormatter(value);
                    SetFieldValueDynamic(_node, _nodeFieldName, value);
                    break;
                }
                else
                {
                    continue;
                }
            }
            return _node;
        }

        #endregion

    }
    public class NodeDes
    {
        public string nodeID = string.Empty;
        public string des = string.Empty;
        public NodeDes(string _id, string _des)
        {
            this.nodeID = _id;
            this.des = _des;
        }

        public static string exportData()
        {
            List<NodeDes> list = new List<NodeDes>
            {
                new NodeDes("2","学15楼"),
                new NodeDes("3","科技楼"),
                new NodeDes("4","科技楼"),
                new NodeDes("5","东体育场"),
                new NodeDes("6","英东楼"),
                new NodeDes("7","科技楼")
            };

            return JsonConvert.SerializeObject(list);
            // [{"nodeID":"2","des":"学15楼"},{"nodeID":"3","des":"科技楼"},{"nodeID":"4","des":"科技楼"},{"nodeID":"5","des":"东体育场"},{"nodeID":"6","des":"英东楼"},{"nodeID":"7","des":"科技楼"}]
        }
        public static List<NodeDes> importData()
        {
            string strReadFilePath1 = @"./config.txt";
            StreamReader srReadFile1 = new StreamReader(strReadFilePath1);
            string strConfig = srReadFile1.ReadToEnd();
            srReadFile1.Close();
            Debug.WriteLine(strConfig);
            //string strConfig = exportData();
            List<NodeDes> nodes = (List<NodeDes>)JsonConvert.DeserializeObject<List<NodeDes>>(strConfig);
            //if (nodes != null)
            //{

            //}
            return nodes;
        }
    }
}
