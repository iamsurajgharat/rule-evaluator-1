using Akka.Actor;
using RuleEvaluator1.Service.Messages;

namespace RuleEvaluator1.Service.Models
{
    public class SaveMetadataRequestState : BaseRequestState<object>
    {
        public SaveMetadataRequestState(string reqId, IActorRef requestor) : base(reqId, requestor)
        {
        }

        public bool Merge(SaveShardMetadataResponse shardResponse)
        {
            return base.Merge(shardResponse.Shard);
        }
    }
}
