using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class EvaluateRulesRequest : BaseRequest
    {
        public List<Record> Records { get; set; }

        public EvaluateShardRulesRequest GetEvaluateShardRulesRequest(int shardNumber)
        {
            return new EvaluateShardRulesRequest
            {
                Id = Id,
                Records = Records,
                Shard = (RuleShard)shardNumber
            };
        }
    }
}
