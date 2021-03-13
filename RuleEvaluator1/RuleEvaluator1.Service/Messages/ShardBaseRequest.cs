using RuleEvaluator1.Service.Models;

namespace RuleEvaluator1.Service.Messages
{
    public class ShardBaseRequest : BaseRequest
    {
        public RuleShard Shard { get; set; }
    }
}
