using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class EvaluateShardRulesResponse : ShardBaseRequest
    {
        public List<object>[] Result { get; set; }
    }
}
