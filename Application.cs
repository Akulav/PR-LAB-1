using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using RestSharp;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Http.Metadata.Providers;
using System.Xml;
using System.Xml.Linq;
using Xceed.Wpf.Toolkit;
using JsonException = System.Text.Json.JsonException;

namespace PR_LAB_1
{
    class Program
    {
        public static string tokenURL = "http://localhost:5000/register";
        public static string token = "";
        public static string homeURL = "http://localhost:5000/home";
        public static List<ManualResetEvent> events = new List<ManualResetEvent>();

        //public static LinkedList<string> links = new LinkedList<string>();
        //public static string[] links = new string[999];
        //public static int index = 0;
        //public static int work_index = 0;

        private static readonly object balanceLock = new object();
        public static List<string> links = new List<string>();
        private static int flag = 0;
        public static int[] processes = new int[999];
        public static int index = 0;
        public static string preventRepeats = "";

        public static int flag1 = 0;
        public static int flag2 = 0;
        public static int flag3 = 0;
        public static int flag4 = 0;


        static async System.Threading.Tasks.Task Main(string[] args)
        {
            //Get the token and wait a bit.
            ThreadPool.QueueUserWorkItem(GetToken);
            Thread.Sleep(500);

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
                    //index++;
                }

            }

         
            
            /*
            while (true) {
              
                var resetEvent = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(arg =>
                {
                    //Console.WriteLine(links.Count);
                    Work(1);
                    resetEvent.Set();
                    
                });
                
                events.Add(resetEvent);

                

            }
            */
             
            //Wait for all threads to finish before continuing further
            //WaitHandle.WaitAll(events.ToArray());
            
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);
            Thread.Sleep(1000);
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);
            ThreadPool.QueueUserWorkItem(Work);



            while (true){
                //Console.WriteLine(flag1);
                if (flag1 == 0)
                {
                    flag1 = 1;
                    ThreadPool.QueueUserWorkItem(Work);
                }

                if (flag2 == 0)
                {
                    flag2 = 2;
                    ThreadPool.QueueUserWorkItem(Work2);
                }

                if (flag3 == 0)
                {
                    flag3 = 3;
                    ThreadPool.QueueUserWorkItem(Work3);
                }

                if (flag4 == 0)
                {
                    flag4 = 4;
                    ThreadPool.QueueUserWorkItem(Work4);
                }
            }
            


            for (int i = 0; i < links.Count - 1; i++)
            {
                Console.WriteLine(links[i]);

            }


        }

        static async void GetToken(Object stateInfo)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(tokenURL);
            dynamic convertedResult = JsonConvert.DeserializeObject(result);
            token = convertedResult.access_token;
        }

        static async void Work2(Object stateInfo)
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
            catch {
                goto endOfFunction;
            }         

            

            if (result.Contains("xml"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchXML; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                        links.Add(matches[i].ToString().Replace("\"", String.Empty));
                        //index++;
                    //}

                }
            killSwitchXML:;
            }

            else if (result.Contains("csv"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchCSV; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                        links.Add(matches[i].ToString().Replace("\"", String.Empty));
                        //index++;
                    //}

                }
            killSwitchCSV:;
            }

            else if (result.Contains("yami"))
            {

            }

            else
            {
                
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchJson; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                        links.Add(matches[i].ToString().Replace("\"", String.Empty));
                        //index++;
                    //}

                }
            killSwitchJson:;

            }
        endOfFunction:;

            lock (balanceLock)
            {
                flag2 = 0;
            }
        }

        static async void Work4(Object stateInfo)
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



            if (result.Contains("xml"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchXML; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchXML:;
            }

            else if (result.Contains("csv"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchCSV; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchCSV:;
            }

            else if (result.Contains("yami"))
            {

            }

            else
            {

                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchJson; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchJson:;

            }
        endOfFunction:;

            lock (balanceLock)
            {
                flag4 = 0;
            }
        }

        static async void Work3(Object stateInfo)
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



            if (result.Contains("xml"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchXML; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchXML:;
            }

            else if (result.Contains("csv"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchCSV; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchCSV:;
            }

            else if (result.Contains("yami"))
            {

            }

            else
            {

                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchJson; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchJson:;

            }
        endOfFunction:;

            lock (balanceLock)
            {
                flag3 = 0;
            }
        }

        static async void Work(Object stateInfo)
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



            if (result.Contains("xml"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchXML; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchXML:;
            }

            else if (result.Contains("csv"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchCSV; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchCSV:;
            }

            else if (result.Contains("yami"))
            {
                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchYAMI; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchYAMI:;
            }

            else
            {

                dynamic convertedResult = JsonConvert.DeserializeObject(result);
                Console.WriteLine(result);
                var reg = new Regex("\".*?\"");
                if (convertedResult.link == null) { goto killSwitchJson; }
                var matches = reg.Matches(convertedResult.link.ToString());

                for (int i = 0; i < 100; i++)
                {
                    if (i == matches.Count) { break; }

                    //if (i % 2 != 0)
                    //{
                    links.Add(matches[i].ToString().Replace("\"", String.Empty));
                    //index++;
                    //}

                }
            killSwitchJson:;

            }
        endOfFunction:;

            lock (balanceLock)
            {
                flag1 = 0;
            }
        }

    }


}
