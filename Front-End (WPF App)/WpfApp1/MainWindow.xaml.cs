using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string username = "Luk";
        public static string token = null;
        WebSocketSharp.WebSocket ws = new WebSocketSharp.WebSocket("ws://127.0.0.1:3213");
        public MainWindow()
        {
            InitializeComponent();
            Dictionary<string, string> getTokenArray = new Dictionary<string, string>
            {
                { "username", username },
                { "action", "getToken" }
            };
            //ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
            string stringjson = JsonConvert.SerializeObject(getTokenArray);
            ws.Send(stringjson);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> sendMessageArray = new Dictionary<string, string>
            {
                { "username", username },
                { "token", token },
                
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
            var action = ParseJSON(e.Data, "action");
            switch (action)
            {
                case "giveToken":
                    var recieved = ParseJSON(e.Data, "token");
                    token = recieved;
                    break;
                case "sendMessage":
                    Application.Current.Dispatcher.Invoke((Action)delegate {
                        ListBoxItem item1 = new ListBoxItem();
                        addmessage(ParseJSON(e.Data, "message") , ParseJSON(e.Data, "username"), "");
                    });
                    
                    
                   
                    break;
                case "error":
                    MessageBox.Show(ParseJSON(e.Data, "error"));
                    break;
            }
        }

        private void addmessage(string msg, string username, string id)
        {
            
            Label messagebox = new Label();
            messagebox.Tag = username;
            messagebox.Content = msg;
            messagebox.Uid = id;
            messagebox.Height = 57;
            messagebox.Width = 324;
            messagebox.SetResourceReference(StyleProperty, "LabelStyle1");
            lsb.Children.Add(messagebox);
        }

        private void Form1_Load(object sender, RoutedEventArgs e)
        {

           
        }
    }
}