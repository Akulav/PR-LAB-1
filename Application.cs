using System;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace BurlacuLabMarkI
{
    public partial class Form1 : Form
    {
        public string sURL = "http://192.168.103.210:5000/register";
        public string token = "";
        public string route2 = "";
        public string route3 = "http://192.168.103.210:5000/home";
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();

            var result = await client.GetStringAsync(sURL);
            label1.Text = result;
            var reg = new Regex("\".*?\"");
            var matches = reg.Matches(result);

            token = matches[1].ToString().Replace("\"",String.Empty);
            //token = matches[1].ToString();
            route2 = matches[3].ToString().Replace("\"", String.Empty);

            
            client.DefaultRequestHeaders.Add("X-Access-Token", token);
            result = await client.GetStringAsync(route3);
            label1.Text = result.ToString();
            
        }
    }
}