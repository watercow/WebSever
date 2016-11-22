using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Net;
using WebServer.HttpServer;
using System.ComponentModel;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
namespace WebServer.HttpServer
{
    public class GetServerInfo
    {
        public static Dictionary<int, string> Content = new Dictionary<int, string>()
        {
             {400,"<html><head><title>400 Bad Request</title></head><body><h1>400 Bad Request</h1><p>HTTP ERROR 400</p></body></html>"},
             {403,"<html><head><title>403 Forbidden</title></head><body><h1>403 Forbidden</h1><p>HTTP ERROR 403</p></body></html>"},
             {404,"<html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1><p>The requested URL was not found on this server.</p></body></html>"},
             {405,"<html><head><title>405 Method Not Allowed</title></head><body><h1>405 YOU SHALL NOT PASS!!</h1><p>HTTP ERROR 405</p></body></html>"},
             {411,"<html><head><title>411 Length Required</title></head><body><h1>411 Length Required</h1><p>HTTP ERROR 411</p></body></html>"},
             {500,"<html><head><title>500 Internal Server Error</title></head><body><h1>500 Internal Server Error</h1><p>HTTP ERROR 500</p></body></html>"},
             {501,"<html><head><title>501 Not Implemented</title></head><body><h1>501 Not Implemented</h1><p>HTTP ERROR 501</p></body></html>"},
             {503,"<html><head><title>503 Service Unavailable</title></head><body><h1>503 Service Unavailable</h1><p>HTTP ERROR 503</p></body></html>"},
             {505,"<html><head><title>505 HTTP Version not supported</title></head><body><h1>505 HTTP Version not supported</h1><p>HTTP ERROR 505</p></body></html>"}
        };

        public static Dictionary<int, string> ReasonPhrase = new Dictionary<int, string>()
        {
             {400,"Bad Request"},
             {403,"Forbidden"},
             {404,"Not Found"},
             {405,"Method Not Allowed"},
             {411,"Length Required"},
             {500,"Internal Server Error"},
             {501,"Not Implemented"},
             {503,"Service Unavailable"},
             {505,"HTTP Version not supported"}
        };

        public static HttpResponse GetInfo(HttpResponse response, int status)
        {
            String Html_Content;
            //获取服务器日期
            DateTime dt = DateTime.Now;
            string Date = dt.GetDateTimeFormats('r')[0].ToString(); 
            //响应报文首行
            response.Version = "HTTP/1.1";
            response.StatusCode = status.ToString();
            response.ReasonPhrase = ReasonPhrase[status];
            //响应首部
            response.Header.Add("Date", Date);
            response.Header.Add("Server", "Niushen/6.6.66(Niuix) DAV/2 mod_jk/1.2.23");
            response.Header.Add("Content-Type", QuickMimeTypeMapper.GetMimeType(".html"));
            Html_Content = Content[status];
            //响应实体
            response.Content = System.Text.Encoding.Default.GetBytes(Html_Content);
            response.Header.Add("Content-Length", response.Content.Length.ToString());
            return response;
        }
    }
}
