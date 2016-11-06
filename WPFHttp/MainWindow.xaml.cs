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
using WebServer.HttpServer;

namespace WpfApplication3
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public HttpServer httpserver = new HttpServer(80, IPAddress.Any);

        public MainWindow()
        {
            InitializeComponent();
        }

        //将16进制转化为Argb的颜色表示
        public Color ToColor(string colorName)
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

        public Button btn_Start;
        public Button btn_Stop;
        public bool Flag_Start = true;
        public bool Flag_Stop = false;
        public bool Flag_Button_double = false;//当多次点击开始时进行的判断

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            if (Flag_Button_double == false)
            {
                Flag_Button_double = true;
                btn_Start = sender as Button;
                HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
                HttpServer.PROTOCOL_VERSION = "HTTP/1.1";
                Binding httpserverBD = new Binding();
                httpserverBD.Source = httpserver;
                Thread ServerThread = new Thread(httpserver.Start);
                ServerThread.Start();
                if (Flag_Start == true)
                {
                    btn_Start.Background = new SolidColorBrush(Colors.Blue);
                    Flag_Start = false;
                }
                else
                {
                    btn_Start.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                    Flag_Start = true;
                }
            }
            else
            {

            }
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            btn_Stop = sender as Button;
            this.httpserver.Close();
            if (Flag_Stop == true)
            {
                btn_Stop.Background = new SolidColorBrush(Colors.Blue);
                Flag_Stop = false;
            }
            else
            {
                btn_Stop.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x74, 0x74, 0x74));
                Flag_Stop = true;
            }
        }

        
    }
}
