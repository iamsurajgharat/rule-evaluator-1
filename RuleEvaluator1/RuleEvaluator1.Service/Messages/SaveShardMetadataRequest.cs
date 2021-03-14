using RuleEvaluator1.Common.Models;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveShardMetadataRequest : ShardBaseRequest
    {
        public RuleMetadata Metadata { get; set; }

        public SaveShardMetadataResponse GetResponse()
        {
            return new SaveShardMetadataResponse
            {
                Id = this.Id,
                Shard = this.Shard
            };
        }
    }
}
