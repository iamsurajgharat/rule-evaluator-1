using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEvaluator1.Service.Messages
{
    public class GetInputRuleRequest : BaseRequest
    {
        public GetShardInputRuleRequest GetShardInputRuleRequest(int shardNumber)
        {
            return new GetShardInputRuleRequest
            {
                Id = Id,
                Shard = (RuleShard)shardNumber
            };
        }
    }
}
