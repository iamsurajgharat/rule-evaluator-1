using Akka.Actor;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Models
{
    public class GetInputRuleRequestState : BaseRequestState<Dictionary<RuleShard, List<InputRule>>>
    {
        public GetInputRuleRequestState(string reqId, IActorRef requestor) : base(reqId, requestor)
        {
            Result = new Dictionary<RuleShard, List<InputRule>>();
        }

        public bool Merge(GetShardInputRuleResponse shardResponse)
        {
            Result[shardResponse.Shard] = shardResponse.Rules;

            return base.Merge(shardResponse.Shard);
        }

        public GetInputRuleResponse CreateFinalResponse()
        {
            return new GetInputRuleResponse
            {
                Id = requestId,
                Result = Result
            };
        }
    }
}
