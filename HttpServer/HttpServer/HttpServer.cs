
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace WebServer.HttpServer
{
    public class HttpServer
    {
        #region GlobalVariables
        public static string PROTOCOL_VERSION { set; get; }
        public static int SERVER_PORT { set; get; }
        public static IPAddress SITE_HOST { set; get; }
        public static string SITE_PATH { set; get; }
        public static int SERVER_MAX_THREADS { set; get; }
        public static string CERT_PATH { set; get; }
        public static X509Certificate SERVER_CERT;
        #endregion

        private TcpListener HttpListener;
        private TcpListener HttpsListener;

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
        //public void Start()
        //{
        //    this.HttpListener = new TcpListener(IPAddress.Any, SERVER_PORT);
        //    this.HttpListener.Start();

        //    ThreadPool.SetMaxThreads(20, 50);

        //    #if CONSOLE_APP
        //        Console.WriteLine("开始Http监听");
        //    #endif

        //    while (true)
        //    {
        //        TcpClient new_client = this.HttpListener.AcceptTcpClient();

        //        IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;
                
        //        Thread thread = new Thread(HttpProcessor.ClientHandler);

        //        #if CONSOLE_APP
        //        Console.WriteLine("--------------------------------");
        //        Console.WriteLine(
        //            "收到Tcp连接请求 {0}: {1}",
        //            clientIP.Address,
        //            clientIP.Port);
        //        Console.WriteLine(
        //            "开始请求处理");
        //        #endif

        //        thread.Start(new_client);
        //    }
        //}

        public void StartSSL()
        {
            this.HttpsListener = new TcpListener(IPAddress.Any, 443);
            this.HttpsListener.Start();
            SERVER_CERT = X509Certificate2.CreateFromCertFile(HttpServer.CERT_PATH);

#if CONSOLE_APP
            Console.WriteLine("开始Https监听");
#endif
            while (true)
            {
                TcpClient new_client = this.HttpsListener.AcceptTcpClient();

                IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;
                
                Thread thread = new Thread(HttpProcessor.ClientHandler);

                #if CONSOLE_APP
                Console.WriteLine("--------------------------------");
                Console.WriteLine(
                    "收到Tcp连接请求 {0}: {1}",
                    clientIP.Address,
                    clientIP.Port);
                Console.WriteLine(
                    "开始请求处理");
                #endif

                thread.Start(new_client);
            }
        }
    }
}
