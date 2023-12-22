using System;
using System.Collections.Generic;
using System.Linq;
using Chamberlain.AppServer.Api.Contracts.Services;
using Chamberlain.Common.Content.Constants;
using Chamberlain.Database.Persistency.Model;
using Chamberlain.ExternalServices.RabbitMq;
using Akka.Actor;
using Chamberlain.Common.Content.Commands;
using PredefinedRulesManager;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Mode;
using Chamberlain.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Chamberlain.AppServer.Api.Services
{
    public class ModeService : IModeService
    {
        private static readonly Object LockObject = new Object();

        public List<ModeModel> GetModes(string userName)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Modes).First(x => x.Username.Equals(userName));

                return customer.Modes.Select(mode => new ModeModel
                    {
                        ModeId = mode.Id,
                        Name = mode.Name,
                        Color = $"#{mode.Color}"
                    })
                    .ToList();
            }
        }

        public ModeModel GetMode(string userName, long modeId)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Modes).First(x => x.Username.Equals(userName));
                var mode = context.Modes.First(m => m.Id == modeId);
                if (mode.CustomerId != customer.Id)
                    throw new ArgumentException("Mode is not valid.");

                var res = new ModeModel
                {
                    ModeId = mode.Id,
                    Name = mode.Name,
                    Color = $"#{mode.Color}"
                };

                return res;
            }
        }

        public void AddMode(string userName, ModePostModel model, IActorRef ruleEngineActorRef)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.First(x => x.Username.Equals(userName));

                var newMode = new Mode
                {
                    CustomerId = customer.Id,
                    Name = model.Name,
                    Color = model.Color
                };
                context.Modes.Add(newMode);
                context.SaveChanges();

                RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSecurityModeChangedMessageType, customer.Id,
                    null, null, string.Empty));
                ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
            }
        }

        public void UpdateMode(string userName, long modeId, string name)
        {
            using (var context = new Entities())
            {
                var customer = context.Customers.Include(x => x.Modes).First(x => x.Username.Equals(userName));
                var mode = customer.Modes.First(m => m.Id == modeId);
                mode.Name = name;
                context.SaveChanges();
            }
        }

        public void UpdateModeColor(string userName, long modeId, string color)
        {
            lock (LockObject)
            {
                using (var context = new Entities())
                {
                    var customer = context.Customers.Include(x => x.Modes).First(x => x.Username.Equals(userName));
                    var mode = customer.Modes.First(m => m.Id == modeId);

                    mode.Color = color.TrimStart('#');

                    context.SaveChanges();
                }
            }
        }

        public void DeleteMode(string userName, long modeId, IActorRef ruleEngineActorRef)
        {
            lock (LockObject)
            {
                using (var context = new Entities())
                {
                    var customer = context.Customers.First(x => x.Username.Equals(userName));
                    if (customer.CurrentMode?.Id == modeId)
                        throw new ArgumentException("Can not remove active mode!");

                    var mode = customer.Modes.FirstOrDefault(m => m.Id == modeId);
                    if (mode == null || mode.CustomerId != customer.Id)
                        throw new ArgumentException("Wrong mode;");

                    var scheduleEntries = context.ScheduleEntries;
                    scheduleEntries.RemoveRange(scheduleEntries.Where(se => se.ModeId == modeId));

                    context.ModeRuleBindings.RemoveRange(context.ModeRuleBindings.Where(i => i.ModeId == mode.Id));
                    context.SaveChanges();

                    CommonQueries.RemoveMode(context, modeId);
                    
                    RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserScheduleChangedMessageType, customer.Id, null, null, string.Empty));
                    RabbitMqSender.SendMessage(new RabbitMqMessage(MessageTypes.UserSecurityModeChangedMessageType, customer.Id, null, null, string.Empty));
                    ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                }
            }
        }
        
        public void Update(string userName, ModeModel model, IActorRef ruleEngineActorRef)
        {
            lock (LockObject)
            {
                using (var context = new Entities())
                {
                    var customer = context.Customers.Include(x => x.Modes).First(x => x.Username.Equals(userName));
                    var result = customer.Modes.First(m => m.Id == model.ModeId);

                    result.Name = model.Name;
                    result.Color = model.Color;
                   
                    try
                    {
                        var original = context.Modes.Find(result.Id);
                        if (original != null)
                        {
                            context.Entry(original).CurrentValues.SetValues(result);
                            context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Could not update mode {result.Id}, {ex.GetDetails()}");
                    }

                    ruleEngineActorRef.Tell(new CustomerRulesChanged(customer.Id));
                }
            }
        }
    }
}