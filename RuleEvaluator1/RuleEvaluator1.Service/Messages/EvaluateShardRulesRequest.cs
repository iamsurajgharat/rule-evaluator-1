using RuleEvaluator1.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Messages
{
    public class EvaluateShardRulesRequest : BaseRequest
    {
        public int ShardNumber { get; set; }
        public List<Record> Records { get; set; }

        public EvaluateShardRulesResponse GetResponse()
        {
            return new EvaluateShardRulesResponse
            {
                Id = Id,
                Result = Enumerable.Range(0, Records.Count).Select(x => new List<object>()).ToArray(),
                ShardNumber = ShardNumber
            };
        }
    }
}
