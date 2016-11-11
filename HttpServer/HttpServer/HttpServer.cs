
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace WebServer.HttpServer
{
    public class HttpServer : INotifyPropertyChanged
    {
        #region GlobalVariables

        public event PropertyChangedEventHandler PropertyChanged;

        public static string PROTOCOL_VERSION { set; get; }
        public static int SERVER_PORT { set; get; }
        public static string SERVER_ADDR { set; get; }
        public static IPAddress SITE_HOST { set; get; }
        public static string SITE_PATH { set; get; }
        public static int SERVER_MAX_THREADS { set; get; }
        public static TcpListener Listener { set; get; }
        public static Thread SERVER_THREAD { set; get; }
        public static bool SERVER_STATUS { set; get; }

        private List<HttpProcessor> proc_record;
        public List<HttpProcessor> PROC_RECORD
        {
            get
            {
                return this.proc_record;
            }
            set
            {
                this.proc_record = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(
                        this,
                        new PropertyChangedEventArgs("PROC_RECORD")
                        );
                }
            }
        }

        #endregion

        //设定监听端口/主机地址
        /// <summary>
        /// 实例化一个Http服务器对象
        /// </summary>
        public HttpServer(int set_port, IPAddress set_addr)
        {
            HttpServer.SERVER_PORT = set_port;
            HttpServer.SITE_HOST = set_addr;
            SERVER_STATUS = false;
            PROC_RECORD = new List<HttpProcessor>();
            proc_record = new List<HttpProcessor>();
        }

        //监听Tcp连接请求
        /// <summary>
        /// 启动Http服务器对象
        /// </summary>
        public void Start()
        {
            SERVER_STATUS = true;
            Listener = new TcpListener(IPAddress.Parse(SERVER_ADDR), SERVER_PORT);
            Listener.Start();

            //ThreadPool.SetMaxThreads(20, 50);

#if CONSOLE_APP
            Console.WriteLine("开始Tcp监听");
#endif

            for (int i = 1; true; i++)
            {
                TcpClient new_client;
                try
                {
                    new_client = Listener.AcceptTcpClient();
                }
                catch
                {
                    break;
                }
                IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;
                HttpProcessor new_proc = new HttpProcessor(new_client);
                new_proc.RemoteIP = ((IPEndPoint)new_client.Client.RemoteEndPoint).Address.ToString();
                new_proc.RemotePort = ((IPEndPoint)new_client.Client.RemoteEndPoint).Port.ToString();

                Thread thread = new Thread(new_proc.ClientHandler);
                thread.Name = "HttpProc #" + i;

                proc_record.Add(new_proc);

#if CONSOLE_APP
                Console.WriteLine("--------------------------------");
                Console.WriteLine(
                    "收到Tcp连接请求 {0}: {1}",
                    clientIP.Address,
                    clientIP.Port);
                Console.WriteLine(
                    "开始请求处理");
#endif

                thread.Start();
            }
        }

        public void Close()
        {
            while (Listener.Pending() == true)
            {
                Thread.Sleep(5);
            }
            Listener.Stop();
            SERVER_STATUS = false;
        }
        public static List<string> GetIP()
        {
            NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            List<string> listIPAddr = new List<string>();
            foreach (NetworkInterface NetworkIntf in NetworkInterfaces)
            {
                string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + NetworkIntf.Id + "\\Connection";
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                if (rk != null)
                {
                    if (NetworkIntf.NetworkInterfaceType == NetworkInterfaceType.Ethernet || NetworkIntf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                        {
                            IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();

                            UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;

                            foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                            {
                                if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    listIPAddr.Add(UnicastIPAddressInformation.Address.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return listIPAddr;
        }
    }
}
