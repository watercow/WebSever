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
        //Get Method
        public static HttpResponse GetMethodHandler(HttpRequest request, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            string encoding = "gzip";
            string Connection_state = "keep-alive";
            string Cache_Control_state = "no-cache";
            response.Header.Add("Cache-Control", Cache_Control_state);
            response.Header.Add("Connection", Connection_state);
            if (request.Header.ContainsKey("Accept-Encoding"))
            {
                response.Header.Add("Content-Encoding", request.Header["Accept-Encoding"]);
            }
            switch (encoding)
            {
                case "gzip":
                    handle.GetHandler(response, request);
                    //response.Header.Add("Content-Type", "text/html");
                    break;
            }
            return response;
        }

        //Post Method
        public static HttpResponse PostMethodHandler(HttpRequest request, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            string encoding = "gzip";
            string Connection_state = "keep-alive";
            string Cache_Control_state = "no-cache";
            response.Header.Add("Cache-Control", Cache_Control_state);
            response.Header.Add("Connection", Connection_state);
            switch(encoding)
            {
                case "gzip":
                    handle.PostHandler(response, request);
                    break;
            }
            return response;
        }

        //Options Method
        public static HttpResponse OptionsMethodHandler(HttpRequest request, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            string Support_Method = "GET/POST/PUT/DELETE/OPTIONS";
            response.Header.Add("Support-Method", Support_Method);
            return response;
        }
        //Put Method
        public static HttpResponse PutMethodHandler(HttpRequest request, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            return response;
        }
        //Delete Method
        public static HttpResponse DeleteMethodHandler(HttpRequest requset, HttpResponse response)
        {
            FileHandler handle = new FileHandler();
            return response;
        }
    }
}
