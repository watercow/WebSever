using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.HttpServer.HttpException
{
    public class HttpException : ApplicationException
    {
        public int status { get; set; }

        public HttpException() { }
        public HttpException(string message) : base(message) { }
        public HttpException(string message, int status) : base(message) { this.status = status; }
        public HttpException(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    //public class IOInterupted : HttpException
    //{
    //    public IOInterupted() { }
    //    public IOInterupted(string message) : base(message) { }
    //    public IOInterupted(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    //}
    //public class NullReference : HttpException
    //{
    //    public NullReference() { }
    //    public NullReference(string message) : base(message) { }
    //    public NullReference(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    //}
}
