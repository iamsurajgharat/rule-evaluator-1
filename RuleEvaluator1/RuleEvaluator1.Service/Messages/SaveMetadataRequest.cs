using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Models;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveMetadataRequest : BaseRequest
    {
        public RuleMetadata Metadata { get; set; }

        public SaveShardMetadataRequest GetSaveShardMetadataRequest(int shardNumber)
        {
            return new SaveShardMetadataRequest
            {
                Id = this.Id,
                Metadata = this.Metadata,
                Shard = (RuleShard)shardNumber
            };
        }
    }
}
