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
using WebServer.HttpServer;

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
                //处理Http request并生成响应头
                HttpResponse response = GetResponse(request);
                //将响应报文response写入outoutStream中
                try
                {
                    WriteResponse(outputStream, response);
                }
                catch (HttpException.IOInterupted ex)
                {
                    Console.WriteLine(ex.Message);
                    client.Close();
                }
                //2016.11.3 19:33 add try-catch for IOInterupt
                outputStream.Flush();

                outputStream.Close();
                outputStream = null;       

                inputStream.Close();
                inputStream = null;

                #region CONSOLE
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
                #endregion
                
                //断开连接
                client.Close();
            }
            #region Http Exception Handler
            catch(HttpException.WrongProtocolVersion ex)
            {
                Console.WriteLine(ex.Message);
                client.Close();
            }
            catch (HttpException.BadRequest ex)
            {
                Console.WriteLine(ex.Message);
                client.Close();
            }
            #endregion
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
                throw new HttpException.BadRequest("400 bad request");
            }
            request.Method = tokens[0];
            request.Uri = tokens[1];
            request.Version = tokens[2];

            if (request.Version != HttpServer.PROTOCOL_VERSION)
            {
                throw new HttpException.WrongProtocolVersion("Wrong HTTP version: " + request.Version);
            }

            Dictionary<string, string> requestHeader = new Dictionary<string, string>();
            while ((thisLine = Readline(inputStream)) != null)
            {
                if (thisLine.Length == 0) { break; }
                
                string pattern = @"^(?<headerName>(\w+-?\w+)+):\s(?<headerValue>[^,\s]*)";  //Header value处排除了','号匹配，这样只会选择第一个属性
                Match header = Regex.Match(thisLine, pattern);
                if (header.Success== false)
                {
                    throw new HttpException.BadRequest("400 bad request: " + thisLine);
                }
                requestHeader.Add(header.Result("${headerName}"), header.Result("${headerValue}"));
            }
            request.Header = requestHeader;

            //接收来自client端的content数据
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
            try
            {
                switch (request.Method)
                {
                    case "GET":
                        HttpMethodHandler.GetMethodHandler(request, response);
                        break;
                    case "POST":
                        HttpMethodHandler.PostMethodHandler(request, response);
                        break;
                    case "OPTIONS":
                        HttpMethodHandler.OptionsMethodHandler(request, response);
                        break;
                    //case "PUT":
                    //    HttpMethodHandler.PutMethodHandler(request, response);
                    //    break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            response.Version = request.Version;


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
            //if(response.Content != null)//增加trycatch避免response.Content为空时Write函数等待的错误----------16.11.2 20：39
            //{
            //    outputStream.Write(response.Content, 0, response.Content.Length);
            //}
            if (response != null)
            {
                try
                {
                    outputStream.Write(response.Content, 0, response.Content.Length);
                }
                catch (HttpException.NullReference ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //2016.11.3 19:52 add try-catch for NullReference
            }

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
