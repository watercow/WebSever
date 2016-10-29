using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.HttpServer;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer.HttpServer test = new HttpServer.HttpServer(80, "127.0.0.2");
            test.baes_path = "Resources";
            Console.WriteLine(
                "Web Server 工作在 {0} : {1}",
                test.server_address,
                test.server_port);
            test.Listen();
        }
    }
}
