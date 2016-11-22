﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Net;
using WebServer.HttpServer;
using HttpServer.HttpServer;
using System.ComponentModel;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;


namespace WebServer.HttpServer
{
    public class HttpProcessor
    {
        public HttpRequest request;
        public HttpResponse response;
        public TcpClient client;
        public string RemoteIP;
        public string RemotePort;

        #region Public Methods
        public HttpProcessor(TcpClient client)
        {
            this.client = client;
        }

        //处理客户端请求
        public void ClientHandler()    //Thread实例使用object对象
        {
            Stream inputStream = client.GetStream();
            Stream outputStream = client.GetStream();
            try
            {
                //读取请求行
                request = GetRequest(inputStream);
                if (request == null)
                    throw new HttpException.HttpException("500 internal server error", 500);

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
                    throw new HttpException.HttpException("500 internal server error", 500);

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
                sslStream.Flush();
                sslStream.Close();

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
            }
            #region Http Exception Handler
            catch (HttpException.HttpException ex)
            {
                DateTime dt = DateTime.Now;
                string Date = dt.GetDateTimeFormats('r')[0].ToString();
                response = new HttpResponse();
                String Html_Content;
                switch (ex.status)
                {
                    case 400:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "400";
                        response.ReasonPhrase = "Bad Request";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>400 Bad Request</title></head><body><h1>400 Bad Request</h1><p>HTTP ERROR 400</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 403:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "403";
                        response.ReasonPhrase = "Forbidden";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>403 Forbidden</title></head><body><h1>403 Forbidden</h1><p>HTTP ERROR 403</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 404:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "404";
                        response.ReasonPhrase = "Not Found";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1><p>The requested URL was not found on this server.</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 405:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "405";
                        response.ReasonPhrase = "Method Not Allowed";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>405 Method Not Allowed</title></head><body><h1>405 Method Not Allowed</h1><p>HTTP ERROR 405</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 411:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "411";
                        response.ReasonPhrase = "Length Required";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>411 Length Required</title></head><body><h1>411 Length Required</h1><p>HTTP ERROR 411</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 500:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "500";
                        response.ReasonPhrase = "Internal Server Error";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>500 Internal Server Error</title></head><body><h1>500 Internal Server Error</h1><p>HTTP ERROR 500</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 501:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "501";
                        response.ReasonPhrase = "Not Implemented";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>501 Not Implemented</title></head><body><h1>501 Not Implemented</h1><p>HTTP ERROR 501</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 503:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "503";
                        response.ReasonPhrase = "Service Unavailable";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>503 Service Unavailable</title></head><body><h1>503 Service Unavailable</h1><p>HTTP ERROR 503</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    case 505:
                        response.Version = "HTTP/1.1";
                        response.StatusCode = "505";
                        response.ReasonPhrase = "HTTP Version not supported";
                        response.Header.Add("Date", Date);
                        response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
                        response.Header.Add("Connection", "Keep-Alive");
                        response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
                        Html_Content = "<html><head><title>505 HTTP Version not supported</title></head><body><h1>505 HTTP Version not supported</h1><p>HTTP ERROR 505</p></body></html>";
                        response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
                        response.Header.Add("Content-Length", response.Content.Length.ToString());
                        WriteResponse(sslStream, response);
                        break;
                    default:
                        break;
                }
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
