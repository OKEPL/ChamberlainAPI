namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Rule
{
    public class RuleNotifierModel : BaseChamberlainModel
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}