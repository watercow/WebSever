
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
    public class HttpServer
    {
        #region GlobalVariables
        public static string PROTOCOL_VERSION { set ; get;}
        public static int SERVER_PORT { set; get; }
        public static IPAddress SITE_HOST { set; get; }
        public static string SITE_PATH { set; get; }
        public static int SERVER_MAX_THREADS { set; get; }
        #endregion

        //public TcpListener Listener;
        //public TcpClient new_client;
        //public Thread thread;
        public bool Flag = true;

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

            while (Flag)
            {
                TcpClient new_client = Listener.AcceptTcpClient();

                IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;

                HttpProcessor proc = new HttpProcessor(new_client);
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
            //this.Listener.EndAcceptTcpClient();    异步
            //Listener.Stop();
            //this.new_client.Close();
            //this.thread.Abort();
            Flag = false;
        }

        
    }
}
