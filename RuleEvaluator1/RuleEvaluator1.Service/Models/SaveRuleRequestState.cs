using Akka.Actor;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Models
{
    public class SaveRuleRequestState
    {
        private readonly Dictionary<string, BaseAckResponse> result;
        private readonly string requestId;

        public int PendingResponseCount { get; set; }
        public IActorRef Requestor { get; set; }
        public bool IsComplete => PendingResponseCount == 0;

        public SaveRuleRequestState(string reqId, IActorRef requestor)
        {
            this.requestId = reqId;
            result = new Dictionary<string, BaseAckResponse>();
            this.Requestor = requestor;
        }

        public bool AddShardResponse(SaveShardRulesResponse response)
        {
            foreach (var item in response.Result)
            {
                result[item.Key] = item.Value;
            }

            PendingResponseCount--;

            return IsComplete;
        }

        public SaveRulesResponse CreateFinalResponse()
        {
            return new SaveRulesResponse
            {
                Id = requestId,
                Result = result
            };
        }
    }
}
