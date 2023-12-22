using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    public class CustomerIfttService : ICustomerIfttService
    {
        public List<IftttModel> GetIfttt(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.IftttKeys).First(x => x.Username.Equals(userName));

                return new List<IftttModel>(customer.IftttKeys.Select(x => new IftttModel
                {
                    Ifttt = x.Key,
                    Alerts = x.AlertActive,
                    Label = x.Label
                }));
            }
        }

        public void AddIfttt(string userName, string ifttt, string label, bool alerts)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.IftttKeys).First(x => x.Username.Equals(userName));

                var existingIfttt = customer.IftttKeys.FirstOrDefault(x => x.Key == ifttt);
                if (existingIfttt == null)
                {
                    context.IftttKeys.Add(new IftttKey
                    {
                        Key = ifttt,
                        AlertActive = alerts,
                        Label = label,
                        CustomerId = customer.Id
                    });
                }
                else
                {
                    existingIfttt.AlertActive = alerts;
                    existingIfttt.Label = label;
                }
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null,
                    null, string.Empty));
            }
        }

        public void DeleteIfttt(string userName, string ifttt)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.IftttKeys).First(x => x.Username.Equals(userName));

                var existingIfttt = customer.IftttKeys.FirstOrDefault(x => x.Key == ifttt);
                if (existingIfttt == null)
                    return;

                customer.IftttKeys.Remove(existingIfttt);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id,
                    null, null, string.Empty));
            }
        }

        public void UpdateIfttts(string userName, List<IftttModel> iftttModelList)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.IftttKeys).First(x => x.Username.Equals(userName));
                var iftttsNewData = new List<IftttModel>(iftttModelList);
                UpdateDataExistingIfttts(customer, iftttsNewData, context);

                while (iftttsNewData.Count > 0)
                    AddNewIfttts(iftttsNewData, context, customer);

                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        private void AddNewIfttts(List<IftttModel> iftttsDataUpdate, Entities context, Customer customer)
        {
            var iftttNewState = iftttsDataUpdate.First();

            var newDefaultIfttt = new IftttKey
            {
                Customer = customer,
                CustomerId = customer.Id,
                Key = iftttNewState.Ifttt,
                Label = iftttNewState.Label
            };

            context.IftttKeys.Add(newDefaultIfttt);
            context.SaveChanges();

            var newIfttt = customer.IftttKeys.Last(ifttt => ifttt.Key == newDefaultIfttt.Key && ifttt.Customer == customer);
            newIfttt.AlertActive = iftttNewState.Alerts;
            iftttsDataUpdate.Remove(iftttNewState);

        }

        private void UpdateDataExistingIfttts(Customer customer, List<IftttModel> iftttsNewData, Entities context)
        {
            foreach (var iftttModel in customer.IftttKeys)
            {
                var iftttNewState = iftttsNewData.Find(ifttt => iftttModel.Key == ifttt.Ifttt);

                if (iftttNewState == null)
                {
                    context.IftttKeys.Remove(iftttModel);
                    continue;
                }

                iftttModel.AlertActive = iftttNewState.Alerts;
                iftttsNewData.Remove(iftttNewState);
            }
        }
    }
}