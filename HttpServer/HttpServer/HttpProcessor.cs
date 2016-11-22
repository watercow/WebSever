using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Authentication;


namespace WebServer.HttpServer
{
    public class HttpProcessor
    {
        #region Properties
        public HttpRequest request;
        public HttpResponse response;
        public TcpClient client;
        public string RemoteIP;
        public string RemotePort;
        #endregion

        #region Public Methods
        public HttpProcessor(TcpClient client)
        {
            this.client = client;
        }

        //处理客户端请求
        public void ClientHandler()
        {
            Stream inputStream = client.GetStream();
            Stream outputStream = client.GetStream();
            try
            {
                //读取请求行
                request = GetRequest(inputStream);
                if (request == null)
                    throw new HttpException.HttpException("400 Bad Request", 400);

                #region Process CGI(PHP) Scripts
                string pattern = @"^(?<uri>\S+\.php)(\?(?<form>\S*))*$";  //提取出CGI动态资源
                Match header = Regex.Match(request.Uri, pattern);
                if (header.Success == true)
                {
                    string uri = header.Result("${uri}");
                    string form = header.Result("${form}");
                    //生成php脚本路径
                    //php解释器不支持..的相对路径
                    string php_uri = FileHandler.ParseUri(uri);
                    response = new HttpResponse();
                    byte[] buffer = CgiHandler.PhpHandler(response, request, php_uri, form);
                    outputStream.Write(buffer, 0, buffer.Length);
                }
                #endregion
                else
                {
                    //处理Http request并生成响应头
                    response = GetResponse(request);
                    //将响应报文response写入outoutStream中
                    WriteResponse(outputStream, response);
                }
                outputStream.Flush();

                outputStream.Close();
                outputStream = null;

                inputStream.Close();
                inputStream = null;
                
            }
            #region Http Exception Handler
            catch (HttpException.HttpException ex)
            {
                HttpResponse temp = new HttpResponse();
                response = GetServerInfo.GetInfo(temp, ex.status);
                WriteResponse(outputStream, response);
            }
            #endregion
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                //断开连接
                client.Close();
                ((IDisposable)client).Dispose();
            }
        }

        public void SSLClientHandler()  

        {
            Stream RawStream = client.GetStream();
            SslStream sslStream = new SslStream(RawStream);

            try
            {
                sslStream.AuthenticateAsServer(HttpServer.SERVER_CERT, false, SslProtocols.Tls, true);

                //读取请求行
                request = GetRequest(sslStream);
                if (request == null)
                    throw new HttpException.HttpException("400 Bad Request", 400);

                #region Process CGI(PHP) Scripts
                string pattern = @"^(?<uri>\S+\.php)(\?(?<form>\S*))*$";  //提取出CGI动态资源
                Match header = Regex.Match(request.Uri, pattern);
                if (header.Success == true)
                {
                    string uri = header.Result("${uri}");
                    string form = header.Result("${form}");
                    //生成php脚本路径
                    //php解释器不支持..的相对路径
                    string php_uri = FileHandler.ParseUri(uri);
                    response = new HttpResponse();
                    byte[] buffer = CgiHandler.PhpHandler(response, request, php_uri, form);
                    sslStream.Write(buffer, 0, buffer.Length);
                }
                #endregion
                else
                {
                    //处理Http request并生成响应头
                    response = GetResponse(request);
                    //将响应报文response写入outoutStream中
                    WriteResponse(sslStream, response);
                }
                //断开连接
                sslStream.Flush();
                sslStream.Close();
            }
            #region Http Exception Handler
            catch (HttpException.HttpException ex)
            {
                HttpResponse temp = new HttpResponse();
                response = GetServerInfo.GetInfo(temp, ex.status);
                WriteResponse(sslStream, response);
            }
            #endregion
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                client.Close();
                ((IDisposable)client).Dispose();
            }
        }
        #endregion

        #region Private Methods
        private static HttpRequest GetRequest(Stream inputStream)
        {
            HttpRequest request = new HttpRequest();
            //读取Http请求首行
            string thisLine = Readline(inputStream);
            string[] tokens = thisLine.Split(' ');
            if (tokens.Length != 3)
            {
                //非法的首行导致Http 400错误
                throw new HttpException.HttpException("400 bad request", 400);
            }
            request.Method = tokens[0];

            if (request.Method == "HEAD" || request.Method == "PUT" || request.Method == "DELETE")
                throw new HttpException.HttpException("501 not implemented", 501);

            request.Uri = tokens[1];
            request.Version = tokens[2];

            if (request.Version != HttpServer.PROTOCOL_VERSION)
            {
                //错误的Http版本导致Http 505错误
                throw new HttpException.HttpException("Wrong HTTP version: " + request.Version, 505);
            }

            //处理Http请求首部
            Dictionary<string, string> requestHeader = new Dictionary<string, string>();
            while ((thisLine = Readline(inputStream)) != null)
            {
                if (thisLine.Length == 0) { break; }
                //使用正则表达式匹配并提取首部信息
                string pattern = @"^(?<headerName>(\w+-?\w+)+):\s(?<headerValue>[^,\s]*)";  //Header value处排除了','号匹配，这样只会选择第一个值
                Match header = Regex.Match(thisLine, pattern);
                if (header.Success == false)
                {
                    //无法正确匹配的首部导致Http 400错误
                    throw new HttpException.HttpException("400 bad request: " + thisLine, 400);
                }
                requestHeader.Add(header.Result("${headerName}"), header.Result("${headerValue}"));
            }
            request.Header = requestHeader;

            //接收来自client端的content数据
            if (request.Header.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(request.Header["Content-Length"]);
                int bytesRead = 0;
                byte[] buffer = new byte[totalBytes];
                while (bytesRead < totalBytes)
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
                    default:
                        break;
                }
            }
            catch (HttpException.HttpException ex)
            {
                throw new HttpException.HttpException(ex.Message, ex.status);
            }
            catch (FileNotFoundException)
            {
                throw new HttpException.HttpException("404 Not Found", 404);
            }
            catch (UnauthorizedAccessException)
            {
                throw new HttpException.HttpException("404 Not Found", 404);
            }
            catch (Exception ex)
            {
                Console.Write(ex.GetType().ToString());
                Console.Write(ex.Message);
                throw new HttpException.HttpException("500 Internal Server Error", 500);
            }

            response.Version = request.Version;
            response.StatusCode = Convert.ToString((int)HttpStatusCode.Ok);
            response.ReasonPhrase = Convert.ToString(HttpStatusCode.Ok.ToString());
            if (response.Content != null)
            {
                response.Header.Add("Content-Length", response.Content.Length.ToString());
            }

            return response;
        }

        private static void WriteResponse(Stream outputStream, HttpResponse response)
        {
            if (response.Content == null)
            {
                response.Content = new byte[] { };
            }
            if (!response.Header.ContainsKey("Content-Type"))
            {
                response.Header["Content-Type"] = "text/html";
            }
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
                if (pt == '\r') { continue; }   //Carriage Return
                if (pt == '\n') { break; }      //Line Feed
                if (pt == -1) { Thread.Sleep(1); continue; }    //等待数据流传输
                data += Convert.ToChar(pt);
            }
            return data;
        }
        #endregion
    }
}
