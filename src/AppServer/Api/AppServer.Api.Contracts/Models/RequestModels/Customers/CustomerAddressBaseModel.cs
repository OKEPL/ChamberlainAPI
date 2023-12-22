namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Customers
{
    #region

    using System.ComponentModel.DataAnnotations;

    #endregion

    public class CustomerAddressBaseModel : BaseChamberlainModel
    {
        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [StringLength(5)]
        public string HouseNumber { get; set; }
        
        public string HouseNumberExtension { get; set; }

        [Required]
        [StringLength(10)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        public string Street { get; set; }
    }
}