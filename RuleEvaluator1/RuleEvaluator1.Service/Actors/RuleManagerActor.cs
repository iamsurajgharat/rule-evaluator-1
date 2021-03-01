using Akka.Persistence;
using Antlr4.Runtime;
using RuleEvaluator1.Parser;
using RuleEvaluator1.Service.Helpers;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using Akka.Actor;
using RuleEvaluator1.Common.Models;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleManagerActor : ReceivePersistentActor
    {
        private readonly IActorProviderService actorProviderService;
        private readonly int numberOfAkkaShards;
        private Dictionary<string, RuleDataType> metadata;

        private readonly Dictionary<string, EvaluateRequestState> evaluateRequestData;

        public override string PersistenceId => "RuleManagerActor101";

        public RuleManagerActor(IActorProviderService actorProviderService)
        {
            this.numberOfAkkaShards = 10;
            this.actorProviderService = actorProviderService ?? throw new ArgumentNullException(nameof(actorProviderService));
            this.metadata = new Dictionary<string, RuleDataType>();
            this.evaluateRequestData = new Dictionary<string, EvaluateRequestState>();

            // process add/update rules request
            Command<SaveRuleRequest>(ProcessSaveRuleRequest);

            // metadata 
            Command<SaveMetadataRequest>(ProcessSaveMetadataRequest);

            // eval
            Command<EvaluateRulesRequest>(ProcessEvaluateRulesRequest);

            Command<EvaluateShardRulesResponse>(ProcessEvaluateShardRulesResponse);
        }

        private void ProcessEvaluateRulesRequest(EvaluateRulesRequest request)
        {
            this.evaluateRequestData[request.Id] = new EvaluateRequestState(request.Records.Count, Sender);

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

        private void ProcessSaveRuleRequest(SaveRuleRequest request)
        {
            var rulesByActorIds = new Dictionary<int, List<ParsedRule>>();
            foreach (var item in request.Rules)
            {
                var parsedRule = Parse(item);
                var idSequence = CommonUtil.GetRuleActorIdSequence(item.Id, numberOfAkkaShards);

                if (!rulesByActorIds.ContainsKey(idSequence))
                {
                    rulesByActorIds.Add(idSequence, new List<ParsedRule>());
                }

                rulesByActorIds[idSequence].Add(parsedRule);
            }

            foreach (var rules in rulesByActorIds)
            {
                var entityId = rules.Value[0].RawRule.Id;
                var message = new SaveParsedRulesRequest
                {
                    Rules = rules.Value,
                    Metadata = this.metadata
                };
                ShardEnvelope request2 = new ShardEnvelope(entityId, message);

                actorProviderService.GetRuleActorShardRegionProxy().Tell(request2, Context.Self);
            }
        }

        private void ProcessSaveMetadataRequest(SaveMetadataRequest request)
        {
            if (request.Metadata == null || request.Metadata.Count == 0)
            {
                Sender.Tell(new BaseAckResponse { Message = Common.Constants.Messages.EmptyRequest, IsSuccess = false });
            }

            this.metadata = request.Metadata;

            // sucess ack
            Sender.Tell(new BaseAckResponse { Message = Common.Constants.Messages.SucessAck, Result = this.metadata });
        }

        private ParsedRule Parse(InputRule rule)
        {
            String input = rule.PredicateExpression;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new Predicate1Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Predicate1Parser parser = new Predicate1Parser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.pexpr();
            var visitor = new MyCustomVisitor2();
            var result = visitor.Visit(tree);
            result.RawRule = rule;
            return result;
        }
    }
}
