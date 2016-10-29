using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;
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
            Console.WriteLine("\n解析请求行...");
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
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Following is the parsed headers:");
                Console.WriteLine("--------------------------------");
                foreach (KeyValuePair<string, string> item in request.Header)
                {
                    Console.WriteLine("{0}: {1}", item.Key, item.Value);
                }

                HttpResponse response = new HttpResponse()
                {
                    Version = "HTTP/1.1",
                    StatusCode = Convert.ToString((int)HttpStatusCode.Ok),
                    ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString()),
                    Header = new Dictionary<string, string>(),
                };

                FileHandler handle = new FileHandler();
                handle.base_path = "..\\..\\Resources";
                response.Content = handle.Handler(request);
                response.Header.Add("Content-Type", "text/html");
                response.Header.Add("Content-Encoding", "UTF-8");

                string responseLine = string.Format("{0} {1} {2}\r\n", response.Version, response.StatusCode, response.ReasonPhrase);
                string headerLine = responseLine + string.Join("\r\n", response.Header.Select(x => string.Format("{0}: {1}", x.Key, x.Value))) + "\r\n\r\n";
                byte[] buffer = Encoding.Default.GetBytes(headerLine);
                outputStream.Write(buffer, 0, buffer.Length);

                outputStream.Write(response.Content, 0, response.Content.Length);

                outputStream.Flush();
                outputStream.Close();
                outputStream = null;

                inputStream.Close();
                inputStream = null;

                //断开连接
                client.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                client.Close();
            }
        }

        #region Private Methods
        private static HttpRequest GetRequest(Stream inputStream, Stream outputStream)
        {
            HttpRequest request = new HttpRequest();

            string thisLine = Readline(inputStream);
            string[] tokens = thisLine.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line: " + thisLine);
            }
            request.Method = tokens[0];
            request.Uri = tokens[1];
            request.Ver = tokens[2];

            Dictionary<string, string> requestHeader = new Dictionary<string, string>();
           while ((thisLine = Readline(inputStream)) != null)
            {
                if (thisLine.Length == 0) { break; }
                
                string pattern = @"^(?<headerName>(\w+-?\w+)+):\s(?<headerValue>[^,\s]*)";
                Match header = Regex.Match(thisLine, pattern);
                if (header.Success== false)
                {
                    throw new Exception("invalid http request header: " + thisLine);
                }
                requestHeader.Add(header.Result("${headerName}"), header.Result("${headerValue}"));
            }
            request.Header = requestHeader;

            if (request.Header.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(request.Header["Content-Length"]);
                int bytesRead = totalBytes;
                byte[] buffer = new byte[totalBytes];

                while(bytesRead < totalBytes)
                {
                    int n = inputStream.Read(buffer, bytesRead, totalBytes - bytesRead);
                    bytesRead += n;
                }
                request.Content = Encoding.ASCII.GetString(buffer);
            }

            return request;
        }

        private static string Readline(Stream stream)
        {
            int pt;
            string data = "";
            while (true)
            {
                pt = stream.ReadByte();
                if (pt == '\r') { continue; }   //Carriage -  Return
                if (pt == '\n') { break; }      //    Line -  Feed
                if (pt == -1) { Thread.Sleep(1); continue; }    //等待数据流传输
                data += Convert.ToChar(pt);
            }
            return data;
        }
        #endregion
    }
}
