using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherMonitorServer;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetNodeList()
        {
            StreamReader sr = new StreamReader("data.txt");

            string xml = sr.ReadToEnd();

            List<Node> list = NodeInfoParser.UpdateNodeInfo(xml);
            Assert.IsTrue(list.Count == 6);

            Node node4 = list.First(_node => _node.nodeId == "4");
            Assert.IsTrue(node4.soilTemperature == "24.125000");
            Assert.IsTrue(node4.soilMoisture == "1.730906");

            Node node3 = list.First(_node => _node.nodeId == "3");
            Assert.IsTrue(node3.humidity == "56.977001");
            Assert.IsTrue(node3.temperature == "26.500000");


            Node node2 = list.First(_node => _node.nodeId == "2");
            Assert.IsTrue(node2.humidity == "50.703850");
            Assert.IsTrue(node2.temperature == "29.200001");
            Assert.IsTrue(node2.Solar == "426.342743");
            Assert.IsTrue(node2.WindMax == "7.242048");
            Assert.IsTrue(node2.WindAvg == "1.288463");
            Assert.IsTrue(node2.WindDirAvg == "131.612900");
            Assert.IsTrue(node2.RainRate == "0.000000");
            Assert.IsTrue(node2.BP == "1009.299988");
            Assert.IsTrue(node2.RainTotal == "40.716202");

            Node node7 = list.First(_node => _node.nodeId == "7");
            Assert.IsTrue(node7.humidity == "49.838657");
            Assert.IsTrue(node7.temperature == "29.309999");
            Assert.IsTrue(node7.Solar == "450.250397");
            Assert.IsTrue(node7.WindMax == "5.431536");
            Assert.IsTrue(node7.WindAvg == "0.933139");
            Assert.IsTrue(node7.WindDirAvg == "161.173019");
            Assert.IsTrue(node7.RainRate == "0.000000");
            Assert.IsTrue(node7.BP == "1010.099976");
            Assert.IsTrue(node7.RainTotal == "33.020000");

            Node node5 = list.First(_node => _node.nodeId == "5");
            Assert.IsTrue(node5.humidity == "51.034473");
            Assert.IsTrue(node5.temperature == "28.500000");

            Node node6 = list.First(_node => _node.nodeId == "6");
            Assert.IsTrue(node6.humidity == "40.267788");
            Assert.IsTrue(node6.temperature == "32.910000");

        }

        [TestMethod]
        public void TestUpdateNode()
        {
            Node node = new Node("1");
            node.temperature = "1";
            node.humidity = "2";
            node.soilMoisture = "3";
            node.soilTemperature = "4";
            node.Solar = "5";

            Node nodeNew = new Node("1");
            nodeNew.temperature = "11";
            nodeNew.humidity = "22";

            Node nodeUpdate = node.UpdateNewNode(nodeNew);

            Assert.IsTrue(nodeNew.temperature == nodeUpdate.temperature);
            Assert.IsTrue(nodeUpdate.humidity == nodeNew.humidity);
            Assert.IsTrue(nodeUpdate.soilMoisture == node.soilMoisture);
            Assert.IsTrue(nodeUpdate.soilTemperature == node.soilTemperature);
            Assert.IsTrue(nodeUpdate.Solar == node.Solar);
        }

        [TestMethod]
        public void TestParseNodeXml_ET22WeatherSensor()
        {
            string xml = "<?xml version=\"1.0\" ?><MotePacket><PacketName>ET22 Weather Sensor</PacketName><NodeId>7.000000</NodeId><Port>4.000000</Port><ParsedDataElement><Name>amType</Name><ConvertedValue>11</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>group</Name><ConvertedValue>83</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>nodeId</Name><SpecialType>nodeid</SpecialType><ConvertedValue>32770</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>socketId</Name><ConvertedValue>52</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>boardId</Name><SpecialType>sensorboardid</SpecialType><ConvertedValue>22</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>packetId</Name><ConvertedValue>0</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindLastCnt</Name><ConvertedValue>2</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindMaxCnt</Name><ConvertedValue>4</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindSampleCnt</Name><ConvertedValue>565</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>DirLastCnt</Name><ConvertedValue>799</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>DirSampleCnt</Name><ConvertedValue>730</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>RainSampleCnt</Name><ConvertedValue>0</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>RainTotalCnt</Name><ConvertedValue>1300</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>TimerCnt</Name><ConvertedValue>489</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>TempCnt</Name><ConvertedValue>6316</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>HumidityCnt</Name><ConvertedValue>1940</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>SolarCnt</Name><ConvertedValue>279</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>UVCnt</Name><ConvertedValue>1023</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>AdcRefCnt</Name><ConvertedValue>391</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>BPCnt</Name><ConvertedValue>10104</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>TempIntCnt</Name><ConvertedValue>275</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindLast</Name><ConvertedValue>3.621024</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindMax</Name><ConvertedValue>7.242048</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindAvg</Name><ConvertedValue>2.091900</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindDir</Name><ConvertedValue>281.173035</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>WindDirAvg</Name><ConvertedValue>256.891510</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>Rain</Name><ConvertedValue>0.000000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>RainRate</Name><ConvertedValue>0.000000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>RainTotal</Name><ConvertedValue>33.020000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>Temp</Name><ConvertedValue>23.410000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>Humidity</Name><ConvertedValue>64.031921</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>Solar</Name><ConvertedValue>523.416077</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>BP</Name><ConvertedValue>1010.400024</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>TempInt</Name><ConvertedValue>27.500000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>DewPoint</Name><ConvertedValue>16.210352</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><internal><nodeId>32770.000000</nodeId><sensorDeviceParentNodeId>7.000000</sensorDeviceParentNodeId><sensorDeviceSubAddress>4.000000</sensorDeviceSubAddress><sensorDeviceSensorId>22.000000</sensorDeviceSensorId><sensorTable>eS2000_sensor_results</sensorTable></internal></MotePacket>";
            Node node = NodeInfoParser.ParseNodeXml_ET22WeatherSensor(xml);

            Assert.IsNotNull(node);
            Assert.IsTrue("523.416077" == node.Solar);
            Assert.IsTrue("7.242048" == node.WindMax);
            Assert.IsTrue("2.091900" == node.WindAvg);
            Assert.IsTrue("256.891510" == node.WindDirAvg);
            Assert.IsTrue("0.000000" == node.RainRate);
            Assert.IsTrue("1010.400024" == node.BP);
            Assert.IsTrue("33.020000" == node.RainTotal);
            Assert.IsTrue("7" == node.nodeId);
        }
        [TestMethod]
        public void TestES1201AmbientTemperatureAndHumiditySensor()
        {
            string xml = "<?xml version=\"1.0\" ?><MotePacket><PacketName>eS1201 Ambient Temperature and Humidity Sensor</PacketName><NodeId>6.000000</NodeId><Port>4.000000</Port><ParsedDataElement><Name>amType</Name><ConvertedValue>11</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>group</Name><ConvertedValue>83</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>nodeId</Name><SpecialType>nodeid</SpecialType><ConvertedValue>32775</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>socketId</Name><ConvertedValue>52</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>boardId</Name><SpecialType>sensorboardid</SpecialType><ConvertedValue>17</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>packetId</Name><ConvertedValue>0</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>temperature</Name><ConvertedValue>31.830000</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>humidity</Name><ConvertedValue>43.816799</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>dewPoint</Name><ConvertedValue>17.977972</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><internal><nodeId>32775.000000</nodeId><sensorDeviceParentNodeId>6.000000</sensorDeviceParentNodeId><sensorDeviceSubAddress>4.000000</sensorDeviceSubAddress><sensorDeviceSensorId>17.000000</sensorDeviceSensorId><sensorTable>eS1201_sensor_results</sensorTable></internal></MotePacket>";
            Node node = NodeInfoParser.ParseNodeXml_eS1201AmbientTemperatureAndHumiditySensor(xml);

            Assert.IsNotNull(node);
            Assert.IsTrue("31.830000" == node.temperature);
            Assert.IsTrue("43.816799" == node.humidity);
            //Assert.IsTrue("17.977972" == node.dewPoint);
            Assert.IsTrue("6" == node.nodeId);
        }
        [TestMethod]
        public void TestParseNodeXml_eS1100SoilMoistureSensorV1()
        {
            // eS1100 Soil Moisture Sensor v1
            string xml = "<?xml version=\"1.0\" ?><MotePacket><PacketName>eS1100 Soil Moisture Sensor v1</PacketName><NodeId>4.000000</NodeId><Port>1.000000</Port><ParsedDataElement><Name>amType</Name><ConvertedValue>11</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>group</Name><ConvertedValue>83</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>nodeId</Name><SpecialType>nodeid</SpecialType><ConvertedValue>32777</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>socketId</Name><ConvertedValue>52</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>boardId</Name><SpecialType>sensorboardid</SpecialType><ConvertedValue>23</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>packetId</Name><ConvertedValue>0</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>soilMoisture</Name><ConvertedValue>0.338956</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><internal><nodeId>32777.000000</nodeId><sensorDeviceParentNodeId>4.000000</sensorDeviceParentNodeId><sensorDeviceSubAddress>1.000000</sensorDeviceSubAddress><sensorDeviceSensorId>23.000000</sensorDeviceSensorId><sensorTable>eS1100_sensor_results</sensorTable></internal></MotePacket>";
            Node node = NodeInfoParser.ParseNodeXml_eS1100SoilMoistureSensorV1(xml);

            Assert.IsNotNull(node);
            Assert.IsTrue("4" == node.nodeId);
            Assert.IsTrue("0.338956" == node.soilMoisture);
        }

        [TestMethod]
        public void TestParseNodeXml_SoilTemperatureSensor()
        {
            // eS1100 Soil Moisture Sensor v1
            string xml = "<?xml version=\"1.0\" ?><MotePacket><PacketName>Soil Temperature Sensor</PacketName><NodeId>4.000000</NodeId><Port>2.000000</Port><ParsedDataElement><Name>amType</Name><ConvertedValue>11</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>group</Name><ConvertedValue>83</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>nodeId</Name><SpecialType>nodeid</SpecialType><ConvertedValue>32778</ConvertedValue><ConvertedValueType>uint16</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>socketId</Name><ConvertedValue>52</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>boardId</Name><SpecialType>sensorboardid</SpecialType><ConvertedValue>188</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>packetId</Name><ConvertedValue>0</ConvertedValue><ConvertedValueType>uint8</ConvertedValueType></ParsedDataElement><ParsedDataElement><Name>temp</Name><ConvertedValue>24.062500</ConvertedValue><ConvertedValueType>float</ConvertedValueType></ParsedDataElement><internal><nodeId>32778.000000</nodeId><sensorDeviceParentNodeId>4.000000</sensorDeviceParentNodeId><sensorDeviceSubAddress>2.000000</sensorDeviceSubAddress><sensorDeviceSensorId>188.000000</sensorDeviceSensorId><sensorTable>eS1500_sensor_results</sensorTable></internal></MotePacket>";
            Node node = NodeInfoParser.ParseNodeXml_SoilTemperatureSensor(xml);

            Assert.IsNotNull(node);
            Assert.IsTrue("4" == node.nodeId);
            Assert.IsTrue("24.062500" == node.soilTemperature);
        }
        [TestMethod]
        public void TestReflection()
        {
            Node node = new Node();

            //node = NodeInfoParser.SetFieldValueDynamic(node, "soilMoisture", "111");
            NodeInfoParser.SetFieldValueDynamic(node, "soilMoisture", "111");
            //string nodeTypeInfo = node.GetType().ToString();
            //Type type = Type.GetType("WeatherMonitorServer.Node");
            //FieldInfo fieldInfo = type.GetField("soilMoisture");
            //fieldInfo.SetValue(node, "111");
            Assert.IsTrue(node.soilMoisture == "111");

        }
    }
}
