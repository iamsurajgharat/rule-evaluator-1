using Akka.Actor;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Models
{
    public class SaveRuleRequestState : BaseRequestState<Dictionary<string, BaseAckResponse>>
    {
        public SaveRuleRequestState(string reqId, IActorRef requestor) : base(reqId, requestor)
        {
            Result = new Dictionary<string, BaseAckResponse>();
        }

        public bool Merge(SaveShardRulesResponse response)
        {
            foreach (var item in response.Result)
            {
                Result[item.Key] = item.Value;
            }

            return base.Merge(response.Shard);
        }

        public SaveRulesResponse CreateFinalResponse()
        {
            return new SaveRulesResponse
            {
                Id = requestId,
                Result = Result
            };
        }
    }
}
