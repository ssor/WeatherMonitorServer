
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

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
        public string RainTotal = string.Empty;
        //public string dewPoint = string.Empty;
        public Node() { }
        public Node(string _id)
        {
            this.nodeId = _id;
        }
        public Node UpdateNewNode(Node _node)
        {

#if DEBUG
            Type type = Type.GetType("WeatherMonitorServer.Node");
            FieldInfo[] fields = type.GetFields();
            fields.All(_field =>
            {
                string newValue = (string)_field.GetValue(_node);
                if (newValue != string.Empty && _field.Name != "nodeId")
                {
                    string currentValue = (string)_field.GetValue(this);
                    Debug.WriteLine("更新项：{0} {1} =>  {2}", _field.Name, currentValue, newValue);

                }
                return true;
            });

#endif

            if (_node.BP == string.Empty && this.BP != string.Empty)
            {

                _node.BP = this.BP;
            }

            if (_node.RainTotal == string.Empty && this.RainTotal != string.Empty)
            {
                _node.RainTotal = this.RainTotal;
            }
            if (_node.temperature == string.Empty && this.temperature != string.Empty)
            {
                _node.temperature = this.temperature;
            }
            if (_node.humidity == string.Empty && this.humidity != string.Empty)
            {
                _node.humidity = this.humidity;
            }

            if (_node.soilMoisture == string.Empty && this.soilMoisture != string.Empty)
            {
                _node.soilMoisture = this.soilMoisture;
            }

            if (_node.soilTemperature == string.Empty && this.soilTemperature != string.Empty)
            {
                _node.soilTemperature = this.soilTemperature;
            }

            if (_node.Solar == string.Empty && this.Solar != string.Empty)
            {
                _node.Solar = this.Solar;
            }

            if (_node.WindMax == string.Empty && this.WindMax != string.Empty)
            {
                _node.WindMax = this.WindMax;
            }

            if (_node.WindAvg == string.Empty && this.WindAvg != string.Empty)
            {
                _node.WindAvg = this.WindAvg;
            }

            if (_node.WindDirAvg == string.Empty && this.WindDirAvg != string.Empty)
            {
                _node.WindDirAvg = this.WindDirAvg;
            }

            if (_node.RainRate == string.Empty && this.RainRate != string.Empty)
            {
                _node.RainRate = this.RainRate;
            }



            return _node;
        }
        public string formatedString()
        {
            return string.Format("id => {0} temp:{1} humi:{2} \r\n soil:{3},{4} \r\n Weather:{5}(Solar),{6}(WindMax),{7}(WindAvg),{8}(WindDirAvg),{9}(RainRate),{10}(BP),{11}(RainTotal)",
                nodeId, temperature, humidity, soilMoisture, soilTemperature, Solar, WindMax, WindAvg, WindDirAvg, RainRate, BP, RainTotal);
        }
    }
    public class NodeInfoParser
    {
        //public static List<Node> NodeList = null;
        public static Dictionary<string, Node> NodeDic = null;
        public static List<Node> UpdateNodeInfo(string _xml)
        {
            if (NodeDic == null)
            {
                NodeDic = new Dictionary<string, Node>();
                //NodeList = new List<Node> { new Node("2"), new Node("3"), new Node("4"), new Node("5"), new Node("6") };
                NodeDic.Add("2", new Node("2"));
                NodeDic.Add("3", new Node("3"));
                NodeDic.Add("4", new Node("4"));
                NodeDic.Add("5", new Node("5"));
                NodeDic.Add("6", new Node("6"));
                NodeDic.Add("7", new Node("7"));
            }
            List<Node> nodeList = ParseXmlToNodeList(_xml);
            if (nodeList.Count > 0)
            {
                nodeList.All(_node =>
                {
                    if (NodeDic.ContainsKey(_node.nodeId))
                    {
                        Node n = NodeDic[_node.nodeId].UpdateNewNode(_node);
                        NodeDic[n.nodeId] = n;
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
                    Debug.WriteLine("---------------------------------------------------");
                    if (n != null)
                    {
                        Debug.WriteLine(n.formatedString());
                    }
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
            ParseNodeValue("Solar", xd, newNode);
            ParseNodeValue("WindMax", xd, newNode);
            ParseNodeValue("WindAvg", xd, newNode);
            ParseNodeValue("WindDirAvg", xd, newNode);
            ParseNodeValue("RainRate", xd, newNode);
            ParseNodeValue("BP", xd, newNode);
            ParseNodeValue("RainTotal", xd, newNode);
            return newNode;
        }

        public static Node ParseNodeXml_SoilTemperatureSensor(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("temp", xd, newNode, "soilTemperature");
            return newNode;
        }

        public static Node ParseNodeXml_eS1100SoilMoistureSensorV1(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
            ParseNodeValue("soilMoisture", xd, newNode);
            return newNode;
        }

        public static Node ParseNodeXml_eS1201AmbientTemperatureAndHumiditySensor(string _xml)
        {
            Node newNode = new Node();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(_xml);
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
        static Node ParseNodeValue(string property, XmlDocument _xd, Node _node, string _nodeFieldName = "")
        {
            if (_node == null)
            {
                _node = new Node();
            }
            if (_nodeFieldName == "") _nodeFieldName = property;
            XmlNode nodeID = _xd.SelectSingleNode("/MotePacket/NodeId");
            string id = nodeID.InnerText;
            _node.nodeId = id.Substring(0, id.IndexOf("."));
            XmlNodeList list = _xd.SelectNodes("/MotePacket/ParsedDataElement");
            foreach (XmlNode xn in list)
            {
                XmlNode nodeName = xn.SelectSingleNode("Name");
                if (nodeName.InnerText == property)
                {
                    XmlNode nodeValue = xn.SelectSingleNode("ConvertedValue");
                    SetFieldValueDynamic(_node, _nodeFieldName, nodeValue.InnerText);
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
}
