using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using LunarLabs;
using LunarLabs.Parser.XML;

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

        public static List<string> dataQueue = new List<string>();

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

                }

            }

            static async void GetToken(Object stateInfo)
            {
                var client = new HttpClient();
                var result = await client.GetStringAsync(tokenURL);
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                token = convertedResult.access_token;
            }

            static async void Work(int flagID)
            {


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
                    dataQueue.Add(convertedResult.data.ToString());
                    Console.WriteLine(convertedResult.data.ToString());
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
                }
            }
        }
    }
}
