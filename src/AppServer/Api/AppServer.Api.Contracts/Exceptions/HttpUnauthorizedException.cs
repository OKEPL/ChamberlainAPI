namespace Chamberlain.AppServer.Api.Contracts.Exceptions
{
    #region

    using System;

    #endregion

    public class HttpUnauthorizedException : Exception
    {
        public HttpUnauthorizedException()
        {
        }

        public HttpUnauthorizedException(string message)
            : base(message)
        {
        }

        public HttpUnauthorizedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}