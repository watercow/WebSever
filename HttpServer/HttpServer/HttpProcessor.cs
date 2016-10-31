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
        #region Public Methods
        //处理客户端请求
        public static void ClientHandler(object oclient)    //Thread实例使用object对象
        {
            TcpClient client = (TcpClient)oclient;
            Stream inputStream = client.GetStream();
            Stream outputStream = client.GetStream();
            try
            {
                //读取请求行
                HttpRequest request = GetRequest(inputStream);

                #if CONSOLE_APP
                Console.WriteLine("\n解析请求行...");
                Console.WriteLine(
                        "Method : {0} \nUri : {1} \nVer : {2} ",
                        request.Method,
                        request.Uri,
                        request.Version
                        );
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Following is the parsed headers:");
                Console.WriteLine("--------------------------------");
                foreach (KeyValuePair<string, string> item in request.Header)
                {
                    Console.WriteLine("{0}: {1}", item.Key, item.Value);
                }
                #endif

                //处理Http request并生成响应头
                HttpResponse response = GetResponse(request);

                WriteResponse(outputStream, response);
                
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
        #endregion

        #region Private Methods
        private static HttpRequest GetRequest(Stream inputStream)
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
            request.Version = tokens[2];

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

        private static HttpResponse GetResponse(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            if (request.Version != HttpServer.PROTOCOL_VERSION) 
            {
                throw new HttpException.Http_InvalidProtocolVersion(null, request.Version);
            }

            response.Version = request.Version;

            response.Header.Add("Content-Type", "text/html");
            response.Header.Add("Content-Encoding", "gzip");

            FileHandler handle = new FileHandler();
            handle.base_path = HttpServer.SITE_PATH;
            response.Content = handle.Handler(request);

            response.StatusCode = Convert.ToString((int)HttpStatusCode.Ok);
            response.ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString());

            return response;
        }

        private static void WriteResponse(Stream outputStream, HttpResponse response)
        {
            string responseLine = string.Format("{0} {1} {2}\r\n", response.Version, response.StatusCode, response.ReasonPhrase);
            string headerLine = responseLine + string.Join("\r\n", response.Header.Select(x => string.Format("{0}: {1}", x.Key, x.Value))) + "\r\n\r\n";
            byte[] buffer = Encoding.Default.GetBytes(headerLine);
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Write(response.Content, 0, response.Content.Length);
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
