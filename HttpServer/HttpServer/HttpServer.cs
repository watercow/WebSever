
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

namespace WebServer.HttpServer
{
    public class HttpServer:INotifyPropertyChanged
    {
        #region GlobalVariables
        public static string PROTOCOL_VERSION { set ; get;}
        public static int SERVER_PORT { set; get; }
        public static IPAddress SITE_HOST { set; get; }
        public static string SITE_PATH { set; get; }
        public static int SERVER_MAX_THREADS { set; get; }

        private static Dictionary<int, HttpProcessor> proc_recode { set; get; }
        public Dictionary<int, HttpProcessor> PROC_RECORD
        {
            get { return proc_recode; }
            set
            {
                proc_recode = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("PROC_RECORD"));
                }
            }
        }

        private Dictionary<int, IPEndPoint> clientip_record { set; get; }
        public  Dictionary<int, IPEndPoint> CLIENTIP_RECORD
        {
            get { return clientip_record; }
            set
            {
                clientip_record = value;
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CLIENTIP_RECORD"));
                }
            }
        }
        #endregion

        //public TcpListener Listener;
        //public TcpClient new_client;
        //public Thread thread;
        public bool Flag = true;

        public event PropertyChangedEventHandler PropertyChanged;

        //设定监听端口/主机地址
        /// <summary>
        /// 实例化一个Http服务器对象
        /// </summary>
        public HttpServer(int set_port, IPAddress set_addr)
        {
            HttpServer.SERVER_PORT = set_port;
            HttpServer.SITE_HOST = set_addr;
        }

        //监听Tcp连接请求
        /// <summary>
        /// 启动Http服务器对象
        /// </summary>
        public void Start()
        {
            Flag = true;
            TcpListener Listener = new TcpListener(IPAddress.Any, SERVER_PORT);
            Listener.Start();

            //ThreadPool.SetMaxThreads(20, 50);

            #if CONSOLE_APP
                Console.WriteLine("开始Tcp监听");
            #endif
            
            for (int i = 1;Flag==true;i++)
            {
                TcpClient new_client = Listener.AcceptTcpClient();
                IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;
                HttpProcessor proc = new HttpProcessor(new_client);

                this.proc_recode.Add(i, proc);
                this.clientip_record.Add(i, clientIP);

                Thread thread = new Thread(proc.ClientHandler);
                //Thread thread = new Thread(HttpProcessor.ClientHandler);

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

            Flag = true;
            Listener.Stop(); 
        }

        public void Close()
        {
            Flag = false;
        }

        
    }
}
