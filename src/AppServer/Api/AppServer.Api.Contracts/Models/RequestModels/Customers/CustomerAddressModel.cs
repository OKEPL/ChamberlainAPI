namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class CustomerAddressModel : CustomerAddressBaseModel
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
    }
}