using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveShardRulesRequest : ShardBaseRequest
    {
        public RuleMetadata Metadata { get; set; }
        public List<InputRule> Rules { get; set; }

        public SaveShardRulesResponse GetSaveShardRulesResponse()
        {
            return new SaveShardRulesResponse 
            { 
                Id = this.Id,
                Result = new Dictionary<string, BaseAckResponse>(),
                Shard = this.Shard
            };
        }
    }
}
