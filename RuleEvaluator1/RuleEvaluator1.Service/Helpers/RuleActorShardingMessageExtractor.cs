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
            return (message is ShardEnvelope msg) && !string.IsNullOrWhiteSpace(msg.entityId)
                ? "Rule_" + CommonUtil.GetRuleActorIdSequence(msg.entityId, numberOfShards)
                : string.Empty;
        }

        public override object EntityMessage(object message) => (message as ShardEnvelope)?.message;
    }
}
