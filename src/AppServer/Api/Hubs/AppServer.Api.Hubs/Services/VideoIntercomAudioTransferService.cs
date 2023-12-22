using System.Linq;
using Chamberlain.AppServer.Api.Hubs.Contracts.Commands;
using Chamberlain.AppServer.Api.Hubs.Contracts.Services;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.Database.Persistency.Model.Enums;

namespace Chamberlain.AppServer.Api.Hubs.Services
{
    public  class VideoIntercomAudioTransferService : IVideoIntercomAudioTransferService
    {
        public  void ReceiveAudioData(AudioDataTransfer.SendAudioData msg)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Single(q => q.Username == msg.UserName);

                var thing = customer.Things.Where(x => x.State == ThingStates.Active)
                    .FirstOrDefault(f => f.NativeKey == msg.DeviceKey);
                if (thing != null)
                {
                    //AG: what next? Check what to do with audio packet (rule engine) and send it to some device or voip gateway...
                }
            }

        }
    }
}
