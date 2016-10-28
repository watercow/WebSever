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
    public class HttpProcessor
    {
        //处理客户端请求
        public static void ClientHandler(object oclient)    //Thread实例使用object对象
        {
            TcpClient client = (TcpClient)oclient;
            Stream inputStream = client.GetStream();
            Stream outputStream = client.GetStream();
            Console.WriteLine("解析请求行...");
            try
            {
                //读取请求行
                HttpRequest request = GetRequest(inputStream, outputStream);

                Console.WriteLine(
                    "Method : {0} \nUri : {1} \nVer : {2} ",
                    request.Method,
                    request.Uri,
                    request.Ver
                    );

                //写响应行
                string response = "HTTP/1.1 200 OK\r\n\r\n";
                outputStream.Write(System.Text.Encoding.ASCII.GetBytes(response), 0, response.Length);
                //响应内容
                byte[] buffer = File.ReadAllBytes(request.Uri);
                outputStream.Write(buffer, 0, buffer.Length);

                //断开连接
                client.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                client.Close();
            }
        }

        private static HttpRequest GetRequest(Stream inputStream, Stream outputStream)
        {
            HttpRequest temp = new HttpRequest();

            string requestLine = Readline(inputStream);

            Console.WriteLine("请求首行 : {0}", requestLine);

            string[] tokens = requestLine.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            temp.Method = tokens[0];
            temp.Uri = tokens[1];
            temp.Ver = tokens[2];
            
            return temp;
        }

        private static string Readline(Stream stream)
        {
            int pt;
            string data = "";
            while (true)
            {
                pt = stream.ReadByte();
                if (pt == '\r') { continue; }
                if (pt == '\n') { break; }
                data += Convert.ToChar(pt);
            }
            return data;
        }
    }
}
