using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace WebServer.App
{
    using WebServer.HttpServer;
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    
    //用来作为datagrid绑定的类
    public class DataGrid_BD
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string Method { get; set; }
        public int Port { get; set; }
        public string Status { get; set; }
        public string URI { get; set; }
        public string Version { get; set; }
        public string Host { get; set; }
        public string User_Agent { get; set; }
        public string Accept { get; set; }
        public string Accept_Encoding { get; set; }
        public DataGrid_BD(int _id, string _ip, string _method, int _port, 
                           string _status, string _uri, string _version, string _host,
                           string _useragent, string _accept, string _acceptencoding)
        {
            Id = _id;
            IP = _ip;
            Method = _method;
            Port = _port;
            Status = _status;
            URI = _uri;
            Version = _version;
            Host = _host;
            User_Agent = _useragent;
            Accept = _accept;
            Accept_Encoding = _acceptencoding;
        }
    }

    public partial class MainWindow : Window
    {
        public HttpServer httpserver;

        

        public MainWindow()
        {
            httpserver = new HttpServer(80, IPAddress.Any);

            //用来保存request.Header字典里的Value
            //Header_BD = new List<string>();

            InitializeComponent();
        }



        private void start_server(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                this.btn_start_server.Background = new SolidColorBrush(Colors.Blue);
                this.btn_stop_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.server_config.IsEnabled = false;

                //Create the main server thread
                Thread ServerThread = new Thread(httpserver.Start);
                ServerThread.Name = "Main Server Thread";

                //Set the default server properties
                HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
                HttpServer.PROTOCOL_VERSION = "HTTP/1.1";
                HttpServer.SERVER_THREAD = ServerThread;

                ServerThread.Start();
            }
            else
            {
                MessageBox.Show("已经有正在运行的服务器例程");
            }
        }

        private void stop_server(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == true)
            {
                this.server_config.IsEnabled = true;
                this.btn_start_server.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                this.btn_stop_server.Background = new SolidColorBrush(Colors.Blue);

                this.httpserver.Close();
            }
            else
            {
                MessageBox.Show("服务器例程已停止");
            }
        }

        private void show_connect(object sender, RoutedEventArgs e)
        {
            if (httpserver.PROC_RECORD.Count == 0)
            {
                MessageBox.Show("尚未有接入的浏览器");
            }
            else
            {
                int now_count = httpserver.PROC_RECORD.Count;
                //作为绑定数据的类的实例----作为数组进行创建
                DataGrid_BD[] datagrid_bd = new DataGrid_BD[now_count];
                for(int j = 0; j < now_count; j++)
                {
                    datagrid_bd[j] = new DataGrid_BD(j, "127.0.0.1", httpserver.PROC_RECORD[j].request.Method, HttpServer.SERVER_PORT, 
                                                    httpserver.PROC_RECORD[j].response.StatusCode + " " + httpserver.PROC_RECORD[j].response.ReasonPhrase,
                                                    httpserver.PROC_RECORD[j].request.Uri, httpserver.PROC_RECORD[j].request.Version, "127.0.0.1", 
                                                    "666",
                                                    "666",
                                                    "666");
                }

                dataGrid.ItemsSource = datagrid_bd;
            }
        }

        private void save_config(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_PORT = Convert.ToInt16(this.tbx_server_port.Text);
                HttpServer.SITE_PATH = this.tbx_site_path.Text;
                HttpServer.SERVER_MAX_THREADS = Convert.ToInt16(this.tbx_thread_max.Text);
                MessageBox.Show(
                    "服务器端口: " + HttpServer.SERVER_PORT + "\n" +
                    "站点路径: " + HttpServer.SITE_PATH + "\n" +
                    "最大线程数: " + HttpServer.SERVER_MAX_THREADS,
                    "修改的服务器配置"
                    );
            }
            else
            {
                MessageBox.Show("服务器运行中");
            }
        }

        private void reset_config(object sender, RoutedEventArgs e)
        {
            if (HttpServer.SERVER_STATUS == false)
            {
                HttpServer.SERVER_PORT = 80;
                HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
                HttpServer.SERVER_MAX_THREADS = 30;
                MessageBox.Show(
                    "服务器端口: " + HttpServer.SERVER_PORT + "\n" +
                    "站点路径: " + HttpServer.SITE_PATH + "\n" +
                    "最大线程数: " + HttpServer.SERVER_MAX_THREADS,
                    "重置的服务器配置"
                    );
            }
            else
            {
                MessageBox.Show("服务器运行中");
            }
        }


        //将16进制转化为Argb的颜色表示
        private Color ToColor(string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = Convert.ToByte((v >> 24) & 255),
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)
            };
        }
    }
}
