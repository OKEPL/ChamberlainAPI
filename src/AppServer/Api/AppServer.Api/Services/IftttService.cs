using System;
using System.Linq;
using System.Threading.Tasks;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Common.Content.DataContracts;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace Chamberlain.AppServer.Api.Services
{
    public class IftttService : IIftttService
    {
        public void TriggerAction(string userLogin, long actionId)
        {
            using (var context = new Entities())
            {
                var rule = context.Rules.Include(x => x.Actions).ThenInclude(x => x.Item).ThenInclude(x => x.Thing).ThenInclude(x => x.Customer).First(bm => bm.Id == actionId);
                var user =
                    rule.Actions.Select(a => a.Item)
                        .Select(i => i.Thing)
                        .Select(t => t.Customer)
                        .First(c => c.Username == userLogin);
                
                //call action
                Log.Debug($"Action '{rule.Name}' triggered by IFTTT");
                var args = JsonConvert.SerializeObject(new RuleTriggeredMessage {RuleName = rule.Name});
                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.RuleTriggeredStartMessageType, user.Id,
                    null, actionId, args, true));

                Task.Run(async () =>
                {
                    await Task.Delay(10000);
                    RabbitMqSender.SendMessage(
                        new RabbitMqMessage(MessageTypes.RuleTriggeredStopMessageType, user.Id, null, actionId, ""));
                });
            }
        }
    }
}
