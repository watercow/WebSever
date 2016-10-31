using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.HttpServer
{
    public enum HttpStatusCode
    {
        Continue = 100,
        Ok = 200,
        Created = 201,
        Accepted = 202,
        MovedPermanently = 301,
        Found = 302,
        NotModified = 304,
        BadRequest = 400,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        InternalServerError = 500
    }

    public class HttpResponse
    {
        public string Version;
        public string StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public Dictionary<string, string> Header { get; set; }
        public byte[] Content { get; set; }
        
        public HttpResponse()
        {
            Header = new Dictionary<string, string>();
        }
    }
}
