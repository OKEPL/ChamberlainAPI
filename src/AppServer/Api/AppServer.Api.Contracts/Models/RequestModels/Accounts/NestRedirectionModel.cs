using System;

namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class NestRedirectionModel
    {
        [Required]
        public Uri RedirectUri { get; set; }
    }
}