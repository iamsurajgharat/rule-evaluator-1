using Akka.Actor;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Models
{
    public class EvaluateRequestState
    {
        public string RequestId { get; set; }
        public List<int> PendingShardNumbers { get; set; }
        public List<object>[] Result { get; set; }
        public IActorRef Requestor { get; set; }

        public bool IsComplete => PendingShardNumbers.Count == 0;

        public EvaluateRequestState(int numberOfRecords, IActorRef requestor)
        {
            Result = Enumerable.Range(0, numberOfRecords).Select(x => new List<object>()).ToArray();
            this.PendingShardNumbers = new List<int>();
            this.Requestor = requestor;
        }

        public bool Merge(EvaluateShardRulesResponse shardRulesResponse)
        {
            for (int i = 0; i < shardRulesResponse.Result.Length; i++)
            {
                Result[i].AddRange(shardRulesResponse.Result[i]);
            }

            PendingShardNumbers.Remove(shardRulesResponse.ShardNumber);

            return IsComplete;
        }

        public EvaluateRulesResponse CreateFinalResponse()
        {
            return new EvaluateRulesResponse
            {
                Id = RequestId,
                Result = Result
            };
        }
    }
}
