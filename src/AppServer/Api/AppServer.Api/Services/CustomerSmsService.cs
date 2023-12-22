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
    public class CustomerSmsService : ICustomerSmsService
    {
        public List<SmsModel> GetSms(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.PhoneNumbers).First(x => x.Username.Equals(userName));
                return new List<SmsModel>(customer.PhoneNumbers.Select(x => new SmsModel
                {
                    PhoneNumber = x.Number,
                    Alerts = x.SmsActive,
                    Voip = x.VoipActive,
                    Label = x.Label
                }));
            }
        }

        public void AddSms(string userName, string sms, string label, bool alerts, bool voip)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.PhoneNumbers).First(x => x.Username.Equals(userName));
                var existingNumber = customer.PhoneNumbers.FirstOrDefault(x => x.Number == sms);
                if (existingNumber == null)
                {
                    context.PhoneNumbers.Add(new PhoneNumber
                    {
                        Number = sms,
                        SmsActive = alerts,
                        VoipActive = voip,
                        Label = label,
                        CustomerId = customer.Id
                    });
                }
                else
                {
                    existingNumber.SmsActive = alerts;
                    existingNumber.VoipActive = voip;
                    existingNumber.Label = label;
                }
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id,
                    null, null, string.Empty));
            }
        }

        public void DeleteSms(string userName, string sms)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.PhoneNumbers).First(x => x.Username.Equals(userName));

                var existingNumber = customer.PhoneNumbers
                    .FirstOrDefault(x => x.Number == sms);
                if (existingNumber == null)
                    return;

                context.PhoneNumbers.Remove(existingNumber);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        public void UpdateSmses(string userName, List<SmsModel> smsModelList)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.PhoneNumbers).First(x => x.Username.Equals(userName));

                var phoneNumbers = context.PhoneNumbers.Where(x => x.CustomerId == customer.Id && !x.IsSecurity);

                foreach (var phoneNumber in phoneNumbers)
                {
                    var phone = smsModelList.FirstOrDefault(x => x.PhoneNumber == phoneNumber.Number);
                    if (phone == null)
                    {
                        context.PhoneNumbers.Remove(phoneNumber);
                        continue;
                    }

                    phoneNumber.FaceProfileId = phone.ProfileId;
                    phoneNumber.SmsActive = phone.Alerts;
                    phoneNumber.VoipActive = phone.Voip;
                }

                smsModelList.ForEach(x =>
                {
                    if (phoneNumbers.ToList().All(y => y.Number != x.PhoneNumber))
                    {
                        context.PhoneNumbers.Add(new PhoneNumber
                        {
                            Number = x.PhoneNumber,
                            SmsActive = x.Alerts,
                            VoipActive = x.Voip,
                            FaceProfileId = x.ProfileId,
                            Label = x.Label,
                            CustomerId = customer.Id,
							IsSecurity = false
                        });
                    }

                    context.SaveChanges();
                    var newPhoneNumberData = customer.PhoneNumbers
                        .Last(nr => nr.CustomerId == customer.Id && nr.Number == x.PhoneNumber  && nr.Label == x.Label);

                    newPhoneNumberData.SmsActive = x.Alerts;
                    newPhoneNumberData.VoipActive = x.Voip;
                });
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }

        public void UpdateSecurityPhones(string userName, List<SecurityPhoneModel> securityPhonesModelList)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var phoneNumbers = context.PhoneNumbers.Where(x => x.CustomerId == customer.Id && x.IsSecurity);

                foreach (var phoneNumber in phoneNumbers)
                {
                    var phone = securityPhonesModelList.FirstOrDefault(x => x.PhoneNumber == phoneNumber.Number);
                    if (phone == null)
                    {
                        context.PhoneNumbers.Remove(phoneNumber);
                        continue;
                    }

                    phoneNumber.SmsActive = phone.Alerts;
                    phoneNumber.VoipActive = phone.Voip;
                }

                securityPhonesModelList.ForEach(x =>
                {
                    if (phoneNumbers.ToList().All(y => y.Number != x.PhoneNumber))
                    {
                        context.PhoneNumbers.Add(new PhoneNumber
                        {
                            Number = x.PhoneNumber,
                            SmsActive = x.Alerts,
                            VoipActive = x.Voip,
                            Label = x.Label,
                            CustomerId = customer.Id,
                            IsSecurity = true,
                        });
                    }
                    context.SaveChanges();
                    var newPhoneNumberData = customer.PhoneNumbers
                        .Last(nr => nr.CustomerId == customer.Id && nr.Number == x.PhoneNumber && nr.Label == x.Label);
                    newPhoneNumberData.SmsActive = x.Alerts;
                    newPhoneNumberData.VoipActive = x.Voip;
                });

                context.SaveChanges();
                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSettingsChangedMessageType, customer.Id, null, null, string.Empty));
            }
        }
    }
}