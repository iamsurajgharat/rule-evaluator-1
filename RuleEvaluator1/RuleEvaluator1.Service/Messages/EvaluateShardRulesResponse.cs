using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class EvaluateShardRulesResponse : BaseRequest
    {
        public int ShardNumber { get; set; }
        public List<object>[] Result { get; set; }
    }
}
