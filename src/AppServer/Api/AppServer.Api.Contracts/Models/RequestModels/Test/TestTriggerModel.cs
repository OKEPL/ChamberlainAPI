namespace Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Test
{
    public class TestTriggerModel
    {
        public TestTriggerModel(string triggerName)
        {
            TriggerName = triggerName;
        }

        public string TriggerName { get; set; }
    }
}