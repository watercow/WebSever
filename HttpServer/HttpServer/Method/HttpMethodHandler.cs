using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer.HttpServer;

namespace WebServer.HttpServer
{
    class HttpMethodHandler
    {
        public enum HttpMethods
        {
            OPTIONS,
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            TRACE,
            CONNECT,
        }
        public static HttpResponse GetMethodHandler(HttpRequest request, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            string encoding = "gzip";
            if (request.Header.ContainsKey("Accept-Encoding"))
            {
                //encoding = ?
            }
            switch (encoding)
            {
                case "gzip":
                    handle.base_path = HttpServer.SITE_PATH;
                    response.Content = handle.Handler(request, "gzip");
                    response.Header.Add("Content-Type", "text/html");
                    response.Header.Add("Content-Encoding", "gzip");
                    break;
            }
            return response;
        }
    }
}
