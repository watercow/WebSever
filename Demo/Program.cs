
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using WebServer.HttpServer;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer test = new HttpServer(80, IPAddress.Any);
            HttpServer.SITE_PATH = "..\\..\\..\\HttpServer\\Resources";
            HttpServer.PROTOCOL_VERSION = "HTTP/1.1";

            Console.WriteLine(
                "Web Server 工作在 本地: {0}端口",
                HttpServer.SERVER_PORT);

            test.Start();
        }
    }
}
