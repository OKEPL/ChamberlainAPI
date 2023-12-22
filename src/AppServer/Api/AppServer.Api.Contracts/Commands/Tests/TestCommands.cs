using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Test;

namespace Chamberlain.AppServer.Api.Contracts.Commands.Tests
{
    public class SimulateFallDetection : HasUserName
    {
        public SimulateFallDetection(string userName)
            : base(userName)
        {
        }
    }
    public class SimulateMotionDetection : HasUserName
    {
        public SimulateMotionDetection(string userName)
            : base(userName)
        {
        }
    }

    public class SimulateTrigger : HasUserName
    {
        public SimulateTrigger(string userName, TestTriggerModel testTriggerModel)
            : base(userName)
        {
            TestTriggerModel = testTriggerModel;
        }
        public TestTriggerModel TestTriggerModel { get; }
    }
}