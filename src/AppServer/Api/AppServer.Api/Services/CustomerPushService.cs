using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    using AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;

    public class CustomerPushService : ICustomerPushService
    {
        public List<FirebasePushModel> GetFirebasePush(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.MobileFirebaseKeys).First(x => x.Username.Equals(userName));

                return new List<FirebasePushModel>(customer.MobileFirebaseKeys.Select(x => new FirebasePushModel
                {
                    Firebase = x.Key,
                    Alerts = x.AlertActive,
                    Label = x.Label
                }));
            }
        }

        public void AddFirebasePush(string userName, string firebase, string label, bool alerts)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.MobileFirebaseKeys)
                    .First(x => x.Username.Equals(userName));

                var existingKey = customer.MobileFirebaseKeys
                    .FirstOrDefault(x => x.Key == firebase);
                if (existingKey == null)
                {
                    context.MobileFirebaseKeys.Add(new MobileFirebaseKey
                    {
                        Key = firebase,
                        AlertActive = alerts,
                        Label = label,
                        CustomerId = customer.Id
                    });
                }
                else
                {
                    existingKey.AlertActive = alerts;
                    existingKey.Label = label;
                }
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null,
                    null, string.Empty));
            }
        }

        public void DeleteFirebasePush(string userName, string firebase)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.MobileFirebaseKeys)
                    .First(x => x.Username.Equals(userName));

                var existingKey = customer.MobileFirebaseKeys
                    .FirstOrDefault(x => x.Key == firebase);
                if (existingKey == null)
                    return;

                context.MobileFirebaseKeys.Remove(existingKey);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id,
                    null, null, string.Empty));
            }
        }
    }
}