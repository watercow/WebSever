using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.HttpServer.HttpException
{
    public class Http_InvalidUri : ApplicationException
    {
        public Http_InvalidUri(string message) : base(message) {  }

    }

    public class Http_BadRequest : ApplicationException
    {
        public Http_BadRequest(string message) : base(message) { }
    }

    public class Http_InvalidProtocolVersion : ApplicationException
    {
        public Http_InvalidProtocolVersion(string message, string version) : base(message + "非法的HTTP协议版本: " + version) { }

    }
}
