
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Microsoft.Win32;
using System.Security.Cryptography.X509Certificates;

namespace WebServer.HttpServer
{
    public class HttpServer : INotifyPropertyChanged
    {
        #region GlobalVariables

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> InnerException;

        public static string PROTOCOL_VERSION { set; get; }

        public static string SERVER_ADDR { set; get; }
        public static int SERVER_PORT { set; get; }
        public static int SERVER_MAX_THREADS { set; get; }
        public static bool SERVER_STATUS { set; get; }

        public static IPAddress SITE_HOST { set; get; }
        public static string SITE_PATH { set; get; }
        public static string SITE_DEFAULT_PAGE { set; get; }

        public static TcpListener Listener { set; get; }
        public static TcpListener HttpsListener { set; get; }

        public static string PHP_PATH { set; get; }

        public static string CERT_PATH { set; get; }
        public static X509Certificate SERVER_CERT;

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
            IPConfig.GetPhysicsNetworkCardIP();
        }

        //监听Tcp连接请求
        /// <summary>
        /// 启动Http服务器对象
        /// </summary>
        public void Start()
        {
            try
            {
                ThreadPool.SetMaxThreads(SERVER_MAX_THREADS, SERVER_MAX_THREADS);

                Listener = new TcpListener(IPAddress.Parse(SERVER_ADDR), SERVER_PORT);
                Listener.Start();
                SERVER_STATUS = true;

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
                    HttpProcessor new_proc = new HttpProcessor(new_client);
                    new_proc.RemoteIP = ((IPEndPoint)new_client.Client.RemoteEndPoint).Address.ToString();
                    new_proc.RemotePort = ((IPEndPoint)new_client.Client.RemoteEndPoint).Port.ToString();

                    Thread thread = new Thread(new_proc.ClientHandler);
                    thread.Name = "HttpProc #" + i;

                    proc_record.Add(new_proc);

                    thread.Start();
                }
            }
            catch(Exception ex)
            {
                InnerException(this, ex.Message);
            }
        }

        public void StartSSL()
        {
            try
            {
                SERVER_CERT = X509Certificate2.CreateFromCertFile(HttpServer.CERT_PATH);

                HttpsListener = new TcpListener(IPAddress.Any, 666);
                HttpsListener.Start();                
                SERVER_STATUS = true;

                for (int i = 1; true; i++)
                {
                    TcpClient new_client;
                    try
                    {
                        new_client = HttpsListener.AcceptTcpClient();
                    }
                    catch
                    {
                        break;
                    }

                    HttpProcessor new_proc = new HttpProcessor(new_client);
                    new_proc.RemoteIP = ((IPEndPoint)new_client.Client.RemoteEndPoint).Address.ToString();
                    new_proc.RemotePort = ((IPEndPoint)new_client.Client.RemoteEndPoint).Port.ToString();

                    Thread thread = new Thread(new_proc.SSLClientHandler);
                    thread.Name = "HttpProc #" + i;

                    proc_record.Add(new_proc);

                    thread.Start();
                }
            }
            catch(Exception ex)
            {
                InnerException(this, ex.Message);
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

        public void CloseSSL()
        {
            while (HttpsListener.Pending() == true)
            {
                Thread.Sleep(5);
            }
            HttpsListener.Stop();
            SERVER_STATUS = false;
        }
    }
}
