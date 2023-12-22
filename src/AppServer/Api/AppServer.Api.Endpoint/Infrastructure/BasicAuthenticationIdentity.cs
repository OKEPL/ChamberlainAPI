namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure
{
    #region

    using System.Security.Principal;

    #endregion

    /// <summary>
    /// The basic authentication identity
    /// </summary>
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        /// <summary>
        /// The basic authentication identity declare user name
        /// </summary>
        public BasicAuthenticationIdentity(string userName)
            : base(userName, "Basic")
        {
            this.UserName = userName;
        }

        /// <summary>
        /// The user id get or set
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user name get or set
        /// </summary>
        public string UserName { get; set; }
    }
}