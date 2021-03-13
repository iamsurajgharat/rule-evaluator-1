using Akka.Actor;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Models
{
    public class BaseRequestState<T>
    {
        protected readonly string requestId;
        public List<RuleShard> PendingShards { get; set; }
        public T Result { get; set; }
        public IActorRef Requestor { get; set; }

        public bool IsComplete => PendingShards.Count == 0;

        public BaseRequestState(string reqId, IActorRef requestor)
        {
            this.PendingShards = new List<RuleShard>();
            this.requestId = reqId;
            this.Requestor = requestor;
        }

        public bool Merge(RuleShard respondedShard)
        {
            PendingShards.Remove(respondedShard);

            return IsComplete;
        }
    }
}
