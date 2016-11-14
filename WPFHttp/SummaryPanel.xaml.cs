using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WebServer.App
{
    using WebServer.HttpServer;
    /// <summary>
    /// SummaryPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SummaryPanel : UserControl
    {
        public string IP { get; set; }
        public string Version { get; set; }
        public string Method { get; set; }
        public string Statue { get; set; }
        public SummaryPanel(string _ip,string _version,string _method,string _statue)
        {
            IP = _ip;
            Version = _version;
            Method = _method;
            Statue = _statue;
        }

        public SummaryPanel()
        {
            InitializeComponent();
        }
    }
}
