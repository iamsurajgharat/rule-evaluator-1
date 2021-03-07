using Akka.Actor;
using Akka.Persistence;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Helpers;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleManagerActor : ReceivePersistentActor
    {
        private readonly IActorProviderService actorProviderService;
        private readonly int numberOfAkkaShards;
        private RuleMetadata metadata;

        private readonly Dictionary<string, EvaluateRequestState> evaluateRequestData;

        // save request state
        private readonly Dictionary<string, SaveRuleRequestState> saveRequestData;

        public override string PersistenceId => "RuleManagerActor101";

        public RuleManagerActor(IActorProviderService actorProviderService)
        {
            this.numberOfAkkaShards = 10;
            this.actorProviderService = actorProviderService ?? throw new ArgumentNullException(nameof(actorProviderService));
            this.metadata = new RuleMetadata();
            this.evaluateRequestData = new Dictionary<string, EvaluateRequestState>();
            this.saveRequestData = new Dictionary<string, SaveRuleRequestState>();

            // process add/update rules request
            Command<SaveRulesRequest>(ProcessSaveRuleRequest);

            // process save response from rule actor
            Command<SaveShardRulesResponse>(ProcessSaveShardRulesResponse);

            // metadata 
            Command<SaveMetadataRequest>(ProcessSaveMetadataRequest);

            // eval
            Command<EvaluateRulesRequest>(ProcessEvaluateRulesRequest);

            // evaluate response from rule actor
            Command<EvaluateShardRulesResponse>(ProcessEvaluateShardRulesResponse);
        }

        private void ProcessEvaluateRulesRequest(EvaluateRulesRequest request)
        {
            this.evaluateRequestData[request.Id] = new EvaluateRequestState(request.Records.Count, request.Id, Sender);

            for (int i = 0; i < numberOfAkkaShards; i++)
            {
                EvaluateShardRulesRequest payload = request.GetEvaluateShardRulesRequest(i);

                ShardEnvelope shardEnvelope = new ShardEnvelope(i.ToString(), payload);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.evaluateRequestData[request.Id].PendingShardNumbers.Add(payload.ShardNumber);
            }
        }

        private void ProcessEvaluateShardRulesResponse(EvaluateShardRulesResponse response)
        {
            if (evaluateRequestData.TryGetValue(response.Id, out var requestState))
            {
                if (requestState.Merge(response))
                {
                    evaluateRequestData.Remove(response.Id);
                    requestState.Requestor.Tell(requestState.CreateFinalResponse(), Self);
                }
            }
            else
            {
                // this should never happen
            }
        }

        private void ProcessSaveRuleRequest(SaveRulesRequest request)
        {
            this.saveRequestData[request.Id] = new SaveRuleRequestState(request.Id, Sender);
            foreach (var rules in request.Rules.GroupBy(x => CommonUtil.GetRuleActorIdSequence(x.Id, numberOfAkkaShards)))
            {
                var entityId = rules.First().Id;
                var message = new SaveShardRulesRequest
                {
                    Id = request.Id,
                    Rules = rules.ToList(),
                    Metadata = this.metadata
                };

                ShardEnvelope shardEnvelope = new ShardEnvelope(entityId, message);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.saveRequestData[request.Id].PendingResponseCount++;
            }
        }

        private void ProcessSaveShardRulesResponse(SaveShardRulesResponse response)
        {
            if (saveRequestData.TryGetValue(response.Id, out var requestState))
            {
                if (requestState.AddShardResponse(response))
                {
                    saveRequestData.Remove(response.Id);
                    requestState.Requestor.Tell(requestState.CreateFinalResponse(), Self);
                }
            }
            else
            {
                // this should never happen
            }
        }

        private void ProcessSaveMetadataRequest(SaveMetadataRequest request)
        {
            if (request.Metadata == null || request.Metadata.IsEmpty())
            {
                Sender.Tell(new BaseAckResponse { Message = Common.Constants.Messages.EmptyRequest, IsSuccess = false });
            }

            this.metadata = request.Metadata;

            // sucess ack
            Sender.Tell(new BaseAckResponse { Message = Common.Constants.Messages.SucessAck, Result = this.metadata });
        }
    }
}
