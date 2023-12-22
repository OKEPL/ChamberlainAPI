using System;

namespace Chamberlain.AppServer.Api.Contracts.Exceptions
{
    public class HttpConflictException : Exception
    {
        public HttpConflictException()
        {
        }

        public HttpConflictException(string message) : base(message)
        {
        }

        public HttpConflictException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
