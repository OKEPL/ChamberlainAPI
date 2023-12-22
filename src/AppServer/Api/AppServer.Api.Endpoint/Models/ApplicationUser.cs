namespace Chamberlain.AppServer.Api.Endpoint.Models
{
    #region

    using System;

    using Microsoft.AspNetCore.Identity;

    #endregion
    /// <summary>
    /// The aplication user 
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The attemps get or set
        /// </summary>
        public int Attemps { get; set; }
        /// <summary>
        /// The pindenail get or set
        /// </summary>
        public DateTime? PinDenail { get; set; }
        /// <summary>
        /// The pin hash get or set
        /// </summary>
        public string PinHash { get; set; }
    }
}