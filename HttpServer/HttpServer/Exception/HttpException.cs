using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.HttpServer.HttpException
{
    public class HttpException : ApplicationException
    {
        public HttpException() { }
        public HttpException(string message) : base(message) { }
        public HttpException(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }

    public class WrongProtocolVersion : HttpException
    {
        public WrongProtocolVersion() { }
        public WrongProtocolVersion(string message) : base(message) { }
        public WrongProtocolVersion(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class NoContent : HttpException
    {
        public NoContent() { }
        public NoContent(string message) : base(message) { }
        public NoContent(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class NotFound : HttpException
    {
        public NotFound() { }
        public NotFound(string message) : base(message) { }
        public NotFound(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class MethodNotAllowed : HttpException
    {
        public MethodNotAllowed() { }
        public MethodNotAllowed(string message) : base(message) { }
        public MethodNotAllowed(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class BadRequest : HttpException
    {
        public BadRequest() { }
        public BadRequest(string message) : base(message) { }
        public BadRequest(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class HTTPVersionNotSupported : HttpException
    {
        public HTTPVersionNotSupported() { }
        public HTTPVersionNotSupported(string message) : base(message) { }
        public HTTPVersionNotSupported(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class IOInterupted : HttpException
    {
        public IOInterupted() { }
        public IOInterupted(string message) : base(message) { }
        public IOInterupted(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
    public class NullReference : HttpException
    {
        public NullReference() { }
        public NullReference(string message) : base(message) { }
        public NullReference(string message, Exception InnerExcepton) : base(message, InnerExcepton) { }

    }
}
