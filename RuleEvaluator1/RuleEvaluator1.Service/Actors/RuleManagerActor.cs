using Akka.Actor;
using RuleEvaluator1.Service.Helpers;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleManagerActor : ReceiveActor
    {
        private readonly IActorProviderService actorProviderService;
        private readonly int numberOfAkkaShards;
        //private RuleMetadata metadata;

        // evaluate request state
        private readonly Dictionary<string, EvaluateRequestState> evaluateRequestData;

        // save rule request state
        private readonly Dictionary<string, SaveRuleRequestState> saveRuleRequestData;

        // save metadata request state
        private readonly Dictionary<string, SaveMetadataRequestState> saveMetadataRequestData;

        // get input request state
        private readonly Dictionary<string, GetInputRuleRequestState> getInputRuleRequestData;

        //public override string PersistenceId => "RuleManagerActor101";

        public RuleManagerActor(IActorProviderService actorProviderService)
        {
            this.numberOfAkkaShards = 10;
            this.actorProviderService = actorProviderService ?? throw new ArgumentNullException(nameof(actorProviderService));
            this.evaluateRequestData = new Dictionary<string, EvaluateRequestState>();
            this.saveRuleRequestData = new Dictionary<string, SaveRuleRequestState>();
            this.saveMetadataRequestData = new Dictionary<string, SaveMetadataRequestState>();
            this.getInputRuleRequestData = new Dictionary<string, GetInputRuleRequestState>();

            // add/update rules request
            Receive<SaveRulesRequest>(ProcessSaveRuleRequest);

            // save response from rule actor
            Receive<SaveShardRulesResponse>(ProcessSaveShardRulesResponse);

            // metadata 
            Receive<SaveMetadataRequest>(ProcessSaveMetadataRequest);

            // metadata save ack from rule actors
            Receive<SaveShardMetadataResponse>(ProcessSaveShardMetadataResponse);

            // eval
            Receive<EvaluateRulesRequest>(ProcessEvaluateRulesRequest);

            // evaluate response from rule actor
            Receive<EvaluateShardRulesResponse>(ProcessEvaluateShardRulesResponse);

            // get input rule request
            Receive<GetInputRuleRequest>(ProcessGetInputRuleRequest);

            // get input rule response from rule actor
            Receive<GetShardInputRuleResponse>(ProcessGetShardInputRuleResponse);
        }

        private void ProcessGetInputRuleRequest(GetInputRuleRequest request)
        {
            this.getInputRuleRequestData[request.Id] = new GetInputRuleRequestState(request.Id, Sender);

            for (int i = 0; i < numberOfAkkaShards; i++)
            {
                GetShardInputRuleRequest payload = request.GetShardInputRuleRequest(i);

                ShardEnvelope shardEnvelope = new ShardEnvelope(i.ToString(), payload);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.getInputRuleRequestData[request.Id].PendingShards.Add(payload.Shard);
            }
        }

        private void ProcessGetShardInputRuleResponse(GetShardInputRuleResponse response)
        {
            if (getInputRuleRequestData.TryGetValue(response.Id, out var requestState))
            {
                if (requestState.Merge(response))
                {
                    getInputRuleRequestData.Remove(response.Id);
                    requestState.Requestor.Tell(requestState.CreateFinalResponse(), Self);
                }
            }
            else
            {
                // this should never happen
            }
        }

        private void ProcessEvaluateRulesRequest(EvaluateRulesRequest request)
        {
            this.evaluateRequestData[request.Id] = new EvaluateRequestState(request.Records.Count, request.Id, Sender);

            for (int i = 0; i < numberOfAkkaShards; i++)
            {
                EvaluateShardRulesRequest payload = request.GetEvaluateShardRulesRequest(i);

                ShardEnvelope shardEnvelope = new ShardEnvelope(i.ToString(), payload);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.evaluateRequestData[request.Id].PendingShards.Add(payload.Shard);
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
            this.saveRuleRequestData[request.Id] = new SaveRuleRequestState(request.Id, Sender);
            foreach (var rules in request.Rules.GroupBy(x => CommonUtil.GetRuleActorIdSequence(x.Id, numberOfAkkaShards)))
            {
                var entityId = rules.First().Id;
                var message = new SaveShardRulesRequest
                {
                    Id = request.Id,
                    Rules = rules.ToList(),
                    Shard = (RuleShard)rules.Key
                };

                ShardEnvelope shardEnvelope = new ShardEnvelope(entityId, message);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.saveRuleRequestData[request.Id].PendingShards.Add(message.Shard);
            }
        }

        private void ProcessSaveShardRulesResponse(SaveShardRulesResponse response)
        {
            if (saveRuleRequestData.TryGetValue(response.Id, out var requestState))
            {
                if (requestState.Merge(response))
                {
                    saveRuleRequestData.Remove(response.Id);
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

            this.saveMetadataRequestData[request.Id] = new SaveMetadataRequestState(request.Id, Sender);

            for (int i = 0; i < numberOfAkkaShards; i++)
            {
                SaveShardMetadataRequest payload = request.GetSaveShardMetadataRequest(i);

                ShardEnvelope shardEnvelope = new ShardEnvelope(i.ToString(), payload);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(shardEnvelope, Context.Self);

                this.saveMetadataRequestData[request.Id].PendingShards.Add(payload.Shard);
            }
        }

        private void ProcessSaveShardMetadataResponse(SaveShardMetadataResponse response)
        {
            if (saveMetadataRequestData.TryGetValue(response.Id, out var requestState))
            {
                if (requestState.Merge(response))
                {
                    saveRuleRequestData.Remove(response.Id);
                    requestState.Requestor.Tell(new BaseAckResponse { Message = Common.Constants.Messages.SucessAck }, Self);
                }
            }
            else
            {
                // this should never happen
            }
        }
    }
}
