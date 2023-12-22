using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.ExternalServices.RabbitMq;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Accounts.Notifications;
using Chamberlain.Database.Persistency.Model;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    public class CustomerEmailService : ICustomerEmailService
    {
        public void DeleteEmail(string userName, string email)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Emails).First(x => x.Username.Equals(userName));

                var existingEmail = customer.Emails.FirstOrDefault(e => e.Address == email);
                if (existingEmail == null)
                    return;

                customer.Emails.Remove(existingEmail);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        public List<EmailModel> GetEmails(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Emails).First(x => x.Username.Equals(userName));

                return new List<EmailModel>(customer.Emails.Select(x => new EmailModel
                {
                    Email = x.Address,
                    Alerts = x.AlertActive,
                    Newsletters = x.NewsletterActive,
                    Label = x.Label
                }));
            }
        }

        public void AddEmail(string userName, string email, bool alerts, bool newsletter)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Emails).First(x => x.Username.Equals(userName));
                var existingEmail = customer.Emails.FirstOrDefault(x => x.Address == email);

                if (existingEmail == null)
                {
                    context.Emails.Add(new Email
                    {
                        CustomerId = customer.Id,
                        Address = email,
                        AlertActive = alerts,
                        NewsletterActive = newsletter,
                        Label = email
                    });
                }
                else
                {
                    existingEmail.AlertActive = alerts;
                    existingEmail.NewsletterActive = newsletter;
                }

                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id,
                    null, null, string.Empty));
            }
        }

        public void UpdateEmails(string userName, List<EmailModel> emailModelNewData)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Emails).First(x => x.Username.Equals(userName));
     
                var emailsNewData = new List<EmailModel>(emailModelNewData);

                UpdateDataExistingEmails(customer, emailsNewData, context);

                while (emailsNewData.Count > 0)
                    AddNewEmails(emailsNewData, context, customer);

                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        private void AddNewEmails(ICollection<EmailModel> emailsDataUpdate, Entities context, Customer customer)
        {
            var emailNewState = emailsDataUpdate.First();

            var newDefaultEmail = new Email
            {
                Customer = customer,
                CustomerId = customer.Id,
                Address = emailNewState.Email,
                Label = emailNewState.Label,
            };

            context.Emails.Add(newDefaultEmail);
            context.SaveChanges();

            var newMailData = customer.Emails.Last(email => email.Address == newDefaultEmail.Address && email.Customer == customer);
            newMailData.AlertActive  = emailNewState.Alerts;
            newMailData.NewsletterActive = emailNewState.Newsletters;
            emailsDataUpdate.Remove(emailNewState);
        }

        private void UpdateDataExistingEmails(Customer customer, List<EmailModel> emailsNewData, Entities context)
        {
            foreach (var customerEmail in customer.Emails)
            {
                var emailNewState = emailsNewData.Find(model => model.Email == customerEmail.Address);

                if (emailNewState == null)
                {
                    context.Emails.Remove(customerEmail);
                    continue;
                }

                customerEmail.AlertActive = emailNewState.Alerts;
                customerEmail.NewsletterActive = emailNewState.Newsletters;
                emailsNewData.Remove(emailNewState);
            }
        }
    }
}