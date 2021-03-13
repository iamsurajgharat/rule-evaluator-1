using RuleEvaluator1.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Messages
{
    public class GetShardInputRuleRequest : ShardBaseRequest
    {
        public GetShardInputRuleResponse CreateResponse(IEnumerable<InputRule> rules)
        {
            return new GetShardInputRuleResponse
            {
                Id = this.Id,
                Rules = rules.ToList(),
                Shard = this.Shard
            };
        }
    }
}
