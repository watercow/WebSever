
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using WebServer.HttpServer;

namespace WebServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer.HttpServer test = new HttpServer.HttpServer(80, IPAddress.Any);
            HttpServer.HttpServer.SITE_PATH = @"C:\Users\xwh16\Desktop\WebSever\HttpServer\Resources";
            HttpServer.HttpServer.PROTOCOL_VERSION = "HTTP/1.1";

            System.Console.WriteLine(
                "Web Server 工作在 本地: {0}端口",
                HttpServer.HttpServer.SERVER_PORT);

            test.Start();
        }
    }
}
