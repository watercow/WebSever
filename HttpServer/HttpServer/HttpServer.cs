using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace WebServer.HttpServer
{
    public class HttpServer
    {
        public int server_port { set; get; }
        public string server_address { set; get; }
        public string baes_path { set; get; }
        public TcpListener Listener;

        //设定监听端口/主机地址
        public HttpServer(int set_port, string set_addr)
        {
            this.server_port = set_port;
            //this.server_address = set_addr;
        }

        //监听Tcp连接请求
        public void Listen()
        {
            this.Listener = new TcpListener(IPAddress.Any, server_port);
            this.Listener.Start();
            Console.WriteLine("开始Tcp监听");
            while (true)
            {
                TcpClient new_client = this.Listener.AcceptTcpClient();

                IPEndPoint clientIP = (IPEndPoint)new_client.Client.RemoteEndPoint;

                Console.WriteLine("--------------------------------");
                Console.WriteLine(
                    "收到Tcp连接请求 {0}: {1}",
                    clientIP.Address,
                    clientIP.Port);

                Thread thread = new Thread(HttpProcessor.ClientHandler);

                Console.WriteLine(
                    "开始请求处理");

                thread.Start(new_client);
            }
        }
    }
}
