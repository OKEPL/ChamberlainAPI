using System;
using System.Collections.Generic;
using Chamberlain.Common.Content.Commands;
using Chamberlain.Hub.HubApp.Contracts.Commands;

namespace Chamberlain.AppServer.Api.Hubs.Contracts.Commands
{
    public class VoiceConfirmationCommands
    {
        public class AskVoiceQuestion : RouteToHubGateway
        {
            public AskVoiceQuestion(DateTime timeStamp, long triggerId, long triggerGroupId, long ruleId,
                long customerId)
            {
                CustomerId = customerId;
                RuleId = ruleId;
                TriggerId = triggerId;
                TriggerGroupId = triggerGroupId;
                TimeStamp = timeStamp;
            }

            public long CustomerId { get; }
            public long RuleId { get; }
            public long TriggerGroupId { get; }
            public long TriggerId { get; }
            public DateTime TimeStamp { get; }

            public string QuestionText { get; set; }

            public List<string> PossibleConfirmationAnswers { get; set; }

            public List<string> PossibleDenialAnswers { get; set; }

            public string ConfirmationInfoMessage { get; set; }

            public string DenialInfoMessage { get; set; }

            public string OriginalSenderPath { get; set; }

            public string VoiceId { get; set; }

            public string ActionOnTimeout { get; set; }
        }

        public class ReceiveVoiceAnswer : TriggerMessage
        {
            public ReceiveVoiceAnswer (bool answeredYes, DateTime timeStamp, long triggerId, long triggerGroupId, long ruleId, long customerId) : base(timeStamp, triggerId, triggerGroupId, ruleId, customerId)
            {
                AnsweredYes = answeredYes;
            }
            public bool AnsweredYes { get; set; }
        }
    }
}
