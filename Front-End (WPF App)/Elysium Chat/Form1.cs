using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Authentication;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace Elysium_Chat
{
    public partial class Form1 : Form
    {
        public static string username = "Luk";
        public static string password = "Sakul04";
        public static string token = null;

        WebSocket ws = new WebSocket("wss://chat.elysium.lukhub.com");



        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> sendMessageArray = new Dictionary<string, string>
            {
                { "username", Form1.username },
                { "token", Form1.token },
                { "message", textBox1.Text },
                { "action", "sendMessage" }
            };


            string stringjson = JsonConvert.SerializeObject(sendMessageArray);
            ws.Send(stringjson);
        }

        public static string ParseJSON(string json, string value)
        {
            JObject val = JObject.Parse(json);
            if (val.GetValue(value) == null)
            {
                return "Invalid";
            }
            return (val.GetValue(value)).ToString();
        }

        private void Ws_OnMessage(object button, MessageEventArgs e)
        {
            Console.WriteLine("Received from the server: " + e.Data);
            var action = ParseJSON(e.Data, "action");
            switch (action)
            {
                case "giveToken":
                    var token = ParseJSON(e.Data, "token");
                    Form1.token = token;
                    break;
                case "sendMessage":
                    richTextBox1.AppendText(ParseJSON(e.Data, "username") + ": " + ParseJSON(e.Data, "message") + "\n");
                    break;
                case "error":
                    MessageBox.Show(ParseJSON(e.Data, "error"));
                    break;
            }
        }

  

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("LMAO");
            Dictionary<string, string> getTokenArray = new Dictionary<string, string>
            {
                { "username", Form1.username },
                { "password", Form1.password },
                { "action", "getToken" }
            };

            ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;


            ws.OnMessage += Ws_OnMessage;

            Console.WriteLine("before");

            ws.Connect();
            Console.WriteLine("after");
            string stringjson = JsonConvert.SerializeObject(getTokenArray);
            ws.Send(stringjson);
        }
    }
}
