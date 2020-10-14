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
using System.Linq;

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

        public static int stopSignal = 0;

        public static List<dynamic> dataQueue = new List<dynamic>();
        public static List<dynamic> csv = new List<dynamic>();
        public static List<dynamic> yaml = new List<dynamic>();
        public static List<dynamic> xml = new List<dynamic>();
        public static List<dynamic> json = new List<dynamic>();

        public static List<dynamic> finalData = new List<dynamic>();

        public static List<dynamic> emails = new List<dynamic>();
        public static List<dynamic> organizations = new List<dynamic>();
        public static List<dynamic> ip_addresses = new List<dynamic>();
        public static List<dynamic> first_names = new List<dynamic>();
        public static List<dynamic> last_names = new List<dynamic>();
        public static List<dynamic> ids = new List<dynamic>();
        public static List<dynamic> usernames = new List<dynamic>();
        public static List<dynamic> created_account_data = new List<dynamic>();
        public static List<dynamic> employee_ids = new List<dynamic>();
        public static List<dynamic> bitcoin_addresses = new List<dynamic>();
        public static List<dynamic> card_numbers = new List<dynamic>();
        public static List<dynamic> card_balances = new List<dynamic>();
        public static List<dynamic> card_currencies = new List<dynamic>();

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


            ThreadPool.QueueUserWorkItem(o => Work(flag1));
            ThreadPool.QueueUserWorkItem(o => Work(flag2));
            ThreadPool.QueueUserWorkItem(o => Work(flag3));
            ThreadPool.QueueUserWorkItem(o => Work(flag4));

            ThreadPool.QueueUserWorkItem(convertToJSON);
            ThreadPool.QueueUserWorkItem(sortData);
            ThreadPool.QueueUserWorkItem(startServer);

            while (true)
            {
                if (stopSignal == 1) { Console.WriteLine("I broke free"); break; }

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


                }

            }

            while (true)
            {
                int a = 0;
                a = a + a;
            }

            static void makeDistinct()
            {
                emails = emails.Distinct().ToList();
                organizations = organizations.Distinct().ToList();
                ip_addresses = ip_addresses.Distinct().ToList();
                first_names = first_names.Distinct().ToList();
                last_names = last_names.Distinct().ToList();
                ids = ids.Distinct().ToList();
                usernames = usernames.Distinct().ToList();
                created_account_data = created_account_data.Distinct().ToList();
                employee_ids = employee_ids.Distinct().ToList();
                bitcoin_addresses = bitcoin_addresses.Distinct().ToList();
                card_numbers = card_numbers.Distinct().ToList();
                card_balances = card_balances.Distinct().ToList();
                card_currencies = card_currencies.Distinct().ToList();
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
                string input = String.Empty;
                while (true)   //we wait for a connection
                {
                    TcpClient client = server.AcceptTcpClient();  //if a connection exists, the server will accept it

                    NetworkStream ns = client.GetStream(); //networkstream is used to send/receive messages

                    byte[] hello = new byte[100];   //any message must be serialized (converted to byte array)
                    hello = Encoding.Default.GetBytes("Africa maladet. Big 'D' to delete input");  //conversion string => byte array

                    ns.Write(hello, 0, hello.Length);     //sending the message



                    while (client.Connected)  //while the client is connected, we look for incoming messages
                    {
                        makeDistinct();

                        stopSignal = 1;

                        try
                        {

                            byte[] msg = new byte[1024];
                            int i = ns.Read(msg, 0, msg.Length);
                            input = input + Encoding.ASCII.GetString(msg, 0, i);



                            if (Encoding.ASCII.GetString(msg, 0, i).Equals("D"))
                            {

                                input = String.Empty;
                                for (int k = 0; k < 128; k++)
                                {
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes("\n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }
                            }

                            if (input.Equals("email"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < emails.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(emails[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("id"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < ids.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(ids[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("username"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < usernames.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(usernames[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("last_name"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < last_names.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(last_names[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("organization"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < organizations.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(organizations[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("created_account_data"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < created_account_data.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(created_account_data[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("ip_address"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < ip_addresses.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(ip_addresses[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("first_name"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < first_names.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(first_names[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }

                            }

                            if (input.Equals("bitcoin_address"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < bitcoin_addresses.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(bitcoin_addresses[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("card_number"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < card_numbers.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(card_numbers[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("card_balance"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < card_balances.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(card_balances[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("card_currency"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < card_currencies.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(card_currencies[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }

                            if (input.Equals("employee_id"))
                            {
                                byte[] output = new byte[100];
                                for (int j = 0; j < employee_ids.Count - 1; j++)
                                {
                                    output = Encoding.Default.GetBytes(employee_ids[j].ToString());
                                    ns.Write(output, 0, output.Length);

                                    input = String.Empty;
                                    byte[] newLine = new byte[100];
                                    newLine = Encoding.Default.GetBytes(" \n");

                                    ns.Write(newLine, 0, newLine.Length);
                                }


                            }
                        }

                        catch { }

                    }

                }

            }

            static void sortData(Object stateInfo)
            {
            beginningSortData:

                if (dataQueue.Count > 0)
                {
                    try
                    {
                        dynamic dynJson = JsonConvert.DeserializeObject(dataQueue[0].ToString());

                        foreach (var item in dynJson)
                        {
                            if (item.ToString().Contains("email"))
                            {
                                //Console.WriteLine(item.email);
                                emails.Add(item.email);
                            }

                            if (item.ToString().Contains("id"))
                            {
                                //Console.WriteLine(item.id);
                                ids.Add(item.id);
                            }

                            if (item.ToString().Contains("username"))
                            {
                                //Console.WriteLine(item.username);
                                usernames.Add(item.username);
                            }

                            if (item.ToString().Contains("created_account_data"))
                            {
                                //Console.WriteLine(item.created_account_data);
                                created_account_data.Add(item.created_account_data);
                            }

                            if (item.ToString().Contains("organization"))
                            {
                                //Console.WriteLine(item.organization);
                                organizations.Add(item.organization);
                            }

                            if (item.ToString().Contains("ip_address"))
                            {
                                //Console.WriteLine(item.ip_address);
                                ip_addresses.Add(item.ip_address);
                            }

                            if (item.ToString().Contains("first_name"))
                            {
                                //Console.WriteLine(item.first_name);
                                first_names.Add(item.first_name);
                            }

                            if (item.ToString().Contains("last_name"))
                            {
                                //Console.WriteLine(item.last_name);
                                last_names.Add(item.last_name);
                            }

                            if (item.ToString().Contains("bitcoin_address"))
                            {
                                //Console.WriteLine(item.last_name);
                                bitcoin_addresses.Add(item.bitcoin_address);
                            }

                            if (item.ToString().Contains("card_number"))
                            {
                                //Console.WriteLine(item.last_name);
                                card_numbers.Add(item.card_number);
                            }

                            if (item.ToString().Contains("card_balance"))
                            {
                                //Console.WriteLine(item.last_name);
                                card_balances.Add(item.card_balance);
                            }

                            if (item.ToString().Contains("card_currency"))
                            {
                                //Console.WriteLine(item.last_name);
                                card_currencies.Add(item.card_currency);
                            }

                            if (item.ToString().Contains("employee_id"))
                            {
                                //Console.WriteLine(item.last_name);
                                employee_ids.Add(item.employee_id);
                            }

                        }

                    }

                    catch { goto beginningSortData; }

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
