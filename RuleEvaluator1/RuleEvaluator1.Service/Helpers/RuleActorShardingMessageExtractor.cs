using Akka.Cluster.Sharding;
using RuleEvaluator1.Service.Messages;

namespace RuleEvaluator1.Service.Helpers
{
    public class RuleActorShardingMessageExtractor : HashCodeMessageExtractor
    {
        private readonly int numberOfShards;
        public RuleActorShardingMessageExtractor(int numberOfShards) : base(numberOfShards)
        {
            this.numberOfShards = numberOfShards;
        }

        public override string EntityId(object message)
        {
            if (message is ShardEnvelope shardEnvelope && shardEnvelope.message is ShardBaseRequest shardBaseRequest)
            {
                return "Rule_" + shardBaseRequest.Shard.Number.ToString();
            }

            return string.Empty;
        }

        public override string ShardId(object message)
        {
            if (message is ShardEnvelope shardEnvelope && shardEnvelope.message is ShardBaseRequest shardBaseRequest)
            {
                return shardBaseRequest.Shard.Number.ToString();
            }

            return base.ShardId(message);
        }

        public override object EntityMessage(object message) => (message as ShardEnvelope)?.message;
    }
}
