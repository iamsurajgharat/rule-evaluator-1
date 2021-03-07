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
            if (message is ShardEnvelope shardEnvelope && !string.IsNullOrWhiteSpace(shardEnvelope.entityId))
            {
                return "Rule_" + (shardEnvelope.message is EvaluateShardRulesRequest evaluateShardRulesRequest ?
                                    CommonUtil.GetRuleActorIdSequence(evaluateShardRulesRequest.ShardNumber, numberOfShards) :
                                    CommonUtil.GetRuleActorIdSequence(shardEnvelope.entityId, numberOfShards)
                                    );
            }

            return string.Empty;
        }

        public override object EntityMessage(object message) => (message as ShardEnvelope)?.message;
    }
}
