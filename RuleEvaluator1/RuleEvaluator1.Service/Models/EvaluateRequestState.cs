using Akka.Actor;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Models
{
    public class EvaluateRequestState : BaseRequestState<List<object>[]>
    {
        public EvaluateRequestState(int numberOfRecords, string reqId, IActorRef requestor) : base(reqId, requestor)
        {
            Result = Enumerable.Range(0, numberOfRecords).Select(x => new List<object>()).ToArray();
        }

        public bool Merge(EvaluateShardRulesResponse shardRulesResponse)
        {
            for (int i = 0; i < shardRulesResponse.Result.Length; i++)
            {
                Result[i].AddRange(shardRulesResponse.Result[i]);
            }

            return base.Merge(shardRulesResponse.Shard);
        }

        public EvaluateRulesResponse CreateFinalResponse()
        {
            return new EvaluateRulesResponse
            {
                Id = requestId,
                Result = Result
            };
        }
    }
}
