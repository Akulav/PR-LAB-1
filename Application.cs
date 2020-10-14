using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using YamlDotNet.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using ChoETL;
using System.Net;
using System.Net.Sockets;

namespace PR_LAB_1
{
    class Program
    {
        public static string tokenURL = "http://localhost:5000/register";
        public static string token = "";
        public static string homeURL = "http://localhost:5000/home";
        public static List<ManualResetEvent> events = new List<ManualResetEvent>();

        private static readonly object balanceLock = new object();
        public static List<string> links = new List<string>();

        private static int flag = 0;

        public static int flag1 = 0;
        public static int flag2 = 0;
        public static int flag3 = 0;
        public static int flag4 = 0;
        public static int flag5 = 0;
        public static int flag6 = 0;
        public static int flag7 = 0;
        public static int flag8 = 0;

        public static List<dynamic> dataQueue = new List<dynamic>();
        public static List<dynamic> csv = new List<dynamic>();
        public static List<dynamic> yaml = new List<dynamic>();
        public static List<dynamic> xml = new List<dynamic>();
        public static List<dynamic> json = new List<dynamic>();

        public static List<dynamic> finalData = new List<dynamic>();

        public static List<dynamic> emails = new List<dynamic>();
        public static List<dynamic> genders = new List<dynamic>();
        public static List<dynamic> ip_addresses = new List<dynamic>();
        public static List<dynamic> first_names = new List<dynamic>();
        public static List<dynamic> last_names = new List<dynamic>();
        public static List<dynamic> ids = new List<dynamic>();
        public static List<dynamic> usernames = new List<dynamic>();
        public static List<dynamic> created_account_data = new List<dynamic>();

        public static string receivedQuery = "";

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //Get the token and wait a bit.
            ThreadPool.QueueUserWorkItem(GetToken);
            Thread.Sleep(1000);

            //Start Getting Data

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Access-Token", token);

            var result = await client.GetStringAsync(homeURL);
            dynamic convertedResult = JsonConvert.DeserializeObject(result);

            var reg = new Regex("\".*?\"");
            var matches = reg.Matches(convertedResult.link.ToString());

            for (int i = 0; i < 100; i++)
            {
                if (i == matches.Count) { break; }

                if (i % 2 != 0)
                {
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                }

            }

            
            ThreadPool.QueueUserWorkItem(sortData);

            ThreadPool.QueueUserWorkItem(o => Work(flag1));
            ThreadPool.QueueUserWorkItem(o => Work(flag2));
            ThreadPool.QueueUserWorkItem(o => Work(flag3));
            ThreadPool.QueueUserWorkItem(o => Work(flag4));

            ThreadPool.QueueUserWorkItem(convertToJSON);
            
            //ThreadPool.QueueUserWorkItem(startServer);
            while (true)
            {
                if (flag1 == 0)
                {
                    {
                        flag1 = 1;
                        ThreadPool.QueueUserWorkItem(o => Work(flag1));
                    }

                    if (flag2 == 0)
                    {
                        flag2 = 2;
                        ThreadPool.QueueUserWorkItem(o => Work(flag2));
                    }

                    if (flag3 == 0)
                    {
                        flag3 = 3;
                        ThreadPool.QueueUserWorkItem(o => Work(flag3));
                    }

                    if (flag4 == 0)
                    {
                        flag4 = 4;
                        ThreadPool.QueueUserWorkItem(o => Work(flag4));
                    }

                    if (flag5 == 0)
                    {
                        flag5 = 5;
                        ThreadPool.QueueUserWorkItem(o => Work(flag5));
                    }

                    if (flag6 == 0)
                    {
                        flag6 = 6;
                        ThreadPool.QueueUserWorkItem(o => Work(flag6));
                    }
   
                    if (flag7 == 0)
                    {
                        flag7 = 7;
                        ThreadPool.QueueUserWorkItem(o => Work(flag7));
                    }

                    if (flag8 == 0)
                    {
                        flag8 = 8;
                        ThreadPool.QueueUserWorkItem(o => Work(flag8));
                    }

                    if(dataQueue.Count == 1)
                    {
                        break;
                    }
                }        
            }

            ThreadPool.QueueUserWorkItem(sortData);
            ThreadPool.QueueUserWorkItem(startServer);

            while (true)
            {
            back:

                goto back;
            }

            static async void GetToken(Object stateInfo)
            {
                var client = new HttpClient();
                var result = await client.GetStringAsync(tokenURL);
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                token = convertedResult.access_token;
            }

            static void startServer(Object stateInfo)
            {

                TcpListener server = new TcpListener(IPAddress.Any, 9999);
                server.Start();

                while (true)   //we wait for a connection
                {
                    TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

                    NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

                    byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
                    hello = Encoding.Default.GetBytes("hello world");  //conversion string => byte array

                    ns.Write(hello, 0, hello.Length);     //sending the message

                    while (client.Connected)  //while the client is connected, we look for incoming messages
                    {
                        
                    }
                }

            }

            static void sortData(Object stateInfo)
            {
            beginningSortData:
                
                if (dataQueue.Count > 0)
                {
                    dynamic dynJson = JsonConvert.DeserializeObject(dataQueue[0].ToString());

                    foreach (var item in dynJson)
                    {
                        if (item.ToString().Contains("email"))
                        {
                            //Console.WriteLine(item.email);
                            emails.Add(item.email);
                        }

                        else if (item.ToString().Contains("id"))
                        {
                            //Console.WriteLine(item.id);
                            ids.Add(item.ids);
                        }

                        else if (item.ToString().Contains("username"))
                        {
                            //Console.WriteLine(item.username);
                            usernames.Add(item.usernames);
                        }

                        else if (item.ToString().Contains("created_account_data"))
                        {
                            //Console.WriteLine(item.created_account_data);
                            created_account_data.Add(item.created_account_data);
                        }

                        if (item.ToString().Contains("gender"))
                        {
                            //Console.WriteLine(item.gender);
                            genders.Add(item.genders);
                        }

                        else if (item.ToString().Contains("ip_address"))
                        {
                            //Console.WriteLine(item.ip_address);
                            ip_addresses.Add(item.ip_addresses);
                        }

                        else if (item.ToString().Contains("first_name"))
                        {
                            //Console.WriteLine(item.first_name);
                            first_names.Add(item.first_names);
                        }

                        else if (item.ToString().Contains("last_name"))
                        {
                            //Console.WriteLine(item.last_name);
                            last_names.Add(item.last_names);
                        }

                    }

                    if (dataQueue.Count > 0)
                    {
                        dataQueue.RemoveAt(0);
                    }
                    
                    
                }

                goto beginningSortData;
            }

            static void convertToJSON(Object stateInfo)
            {
                begginingOfConvert:
                if (xml.Count > 0)
                    {
                       
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xml[0].ToString());
                        string result = JsonConvert.SerializeXmlNode(doc);

                        dynamic convertedResult = JsonConvert.DeserializeObject(result);

                        Console.WriteLine(convertedResult.dataset.record);
                        dataQueue.Add(convertedResult.dataset.record.ToString());
                        xml.RemoveAt(0);
                    }

                if (csv.Count > 0)
                {

                    StringBuilder sb = new StringBuilder();
                    using (var p = ChoCSVReader.LoadText(csv[0].ToString())
                        .WithFirstLineHeader()
                        )
                    {
                        using (var w = new ChoJSONWriter(sb))
                            w.Write(p);
                        dataQueue.Add(sb.ToString());
                    }

                    Console.WriteLine(sb.ToString());
                    csv.RemoveAt(0);

                }

                if (yaml.Count > 0)
                {
                    var r = new StringReader(yaml[0].ToString());
                    var deserializer = new Deserializer();
                    var yamlObject = deserializer.Deserialize(r);

                    var serializer = new Serializer(SerializationOptions.JsonCompatible);
                    using (StringWriter textWriter = new StringWriter())
                    {
                        serializer.Serialize(textWriter, yamlObject);
                        dataQueue.Add(textWriter.ToString());
                        Console.WriteLine(textWriter.ToString());
                    }
                        
                    yaml.RemoveAt(0);

                }

                if (json.Count > 0)
                {
                    Console.WriteLine(json[0]);
                    dataQueue.Add(json[0].ToString());
                    json.RemoveAt(0);
                }
            
                goto begginingOfConvert;
                
            }

            static async void Work(int flagID)
            {

                startOfFunction:

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Access-Token", token);

                lock (balanceLock)
                {

                    if (flag == 0) { flag = 1; goto dontRemove; }
                    try
                    {
                        links.RemoveAt(0);
                    }
                    catch { goto endOfFunction; }

                }
            dontRemove:
                var result = "";
                try
                {
                    result = await client.GetStringAsync("http://localhost:5000" + links[0]);
                }
                catch
                {
                    goto endOfFunction;
                }

                dynamic convertedResult = JsonConvert.DeserializeObject(result);

                Thread.Sleep(1);
                
                lock (balanceLock)
                {
                    if (convertedResult.ToString().Contains("xml"))
                    {
                        xml.Add(convertedResult.data);
                    }

                    else if (convertedResult.ToString().Contains("yaml"))
                    {
                        yaml.Add(convertedResult.data);
                    }

                    else if (convertedResult.ToString().Contains("csv"))
                    {
                        csv.Add(convertedResult.data);
                    }

                    else
                    {
                        json.Add(convertedResult.data);
                    }
                    
                }
                

                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchJson; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                }
            killSwitchJson:;


            endOfFunction:;

                lock (balanceLock)
                {
                    if (flagID == flag1)
                    {
                        flag1 = 0;
                    }
                    if (flagID == flag2)
                    {
                        flag2 = 0;
                    }
                    if (flagID == flag3)
                    {
                        flag3 = 0;
                    }
                    if (flagID == flag4)
                    {
                        flag4 = 0;
                    }
                    if (flagID == flag5)
                    {
                        flag5 = 0;
                    }
                    if (flagID == flag6)
                    {
                        flag6 = 0;
                    }
                    if (flagID == flag7)
                    {
                        flag7 = 0;
                    }
                    if (flagID == flag8)
                    {
                        flag8 = 0;
                    }
                }
            }
        }
    }
}
