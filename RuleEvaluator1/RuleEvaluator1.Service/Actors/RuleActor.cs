using Akka.Persistence;
using Antlr4.Runtime;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Parser;
using RuleEvaluator1.Service.Events;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleActor : ReceivePersistentActor
    {
        //private readonly Dictionary<string, ParsedRule> rules;
        private readonly Dictionary<string, CompiledRule> compiledRules;
        private RuleMetadata metadata;
        private RuleShard ruleShard;
        private SaveShardMetadataRequest shardMetadataRequest;

        public override string PersistenceId => $"{Context.Parent.Path.Name}/{Context.Self.Path.Name}";

        public RuleActor()
        {
            //rules = new Dictionary<string, ParsedRule>();
            compiledRules = new Dictionary<string, CompiledRule>();

            // save metadata
            Command<SaveShardMetadataRequest>(ProcessSaveShardMetadataRequest);

            // process add/update rules request
            Command<SaveShardRulesRequest>(ProcessSaveShardRulesRequest);

            // evaluate
            Command<EvaluateShardRulesRequest>(ProcessEvaluateShardRulesRequest);

            // get rules back
            Command<GetShardInputRuleRequest>(ProcessGetInputRulesRequest);

            Command<object>(x =>
            {
                Context.System.Log.Warning("Unhandled message :" + x);
            });

            // recover
            Recover<RuleAddedOrUpdatedEvent>(ProcessRuleAddedOrUpdatedEvent);
            Recover<RuleMetadataUpsertedEvent>(ProcessRuleMetadataUpsertedEvent);

        }

        private void ProcessSaveShardMetadataRequest(SaveShardMetadataRequest request)
        {
            // save journal
            RuleMetadataUpsertedEvent eventData = new RuleMetadataUpsertedEvent { Metadata = request.Metadata };
            Persist(eventData, ProcessRuleMetadataUpsertedEvent);

            shardMetadataRequest = request;
        }

        private void ProcessGetInputRulesRequest(GetShardInputRuleRequest request)
        {
            Sender.Tell(request.CreateResponse(compiledRules.Values.Select(x => x.RawRule)), Self);
        }

        private void ProcessEvaluateShardRulesRequest(EvaluateShardRulesRequest request)
        {
            EvaluateShardRulesResponse response = request.GetResponse();
            foreach (var rule in compiledRules)
            {
                for (int i = 0; i < request.Records.Count; i++)
                {
                    string result = rule.Value.EvaluateForResult(request.Records[i]);
                    if (result != null)
                    {
                        response.Result[i].Add(result);
                    }
                }
            }

            // send responde back
            Sender.Tell(response, Self);
        }

        private void ProcessSaveShardRulesRequest(SaveShardRulesRequest request)
        {
            SaveShardRulesResponse response = request.GetSaveShardRulesResponse();
            List<ParsedRule> parsedRules = new List<ParsedRule>();
            //List<CompiledRule> compiedRules = new List<CompiledRule>();
            foreach (InputRule item in request.Rules)
            {
                try
                {
                    ParsedRule parsedRule = Parse(this.metadata, item);
                    CompiledRule compiledRule = parsedRule.Compile();

                    // 
                    parsedRules.Add(parsedRule);
                    //compiedRules.Add(compiledRule);

                    //rules[item.Id] = parsedRule;
                    compiledRules[compiledRule.RawRule.Id] = compiledRule;
                    response.Result[item.Id] = new BaseAckResponse();
                }
                catch (RuleEvaluatorException ex)
                {
                    response.Result[item.Id] = new BaseAckResponse
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };
                }
            }

            // save snapshot or journal
            // save journal
            var eventData = new RuleAddedOrUpdatedEvent { ParsedRules = parsedRules };
            Persist(eventData, x => { });

            // save snapshot if 
            //if(SnapshotSequenceNr % 100 ==0)

            // send ack to the manager
            Sender.Tell(response, Self);
        }

        private void ProcessRuleAddedOrUpdatedEvent(RuleAddedOrUpdatedEvent eventData)
        {
            foreach (var parsedRule in eventData.ParsedRules)
            {
                CompiledRule compiledRule = parsedRule.Compile();
                compiledRules[compiledRule.RawRule.Id] = compiledRule;
            }
        }

        private void ProcessRuleMetadataUpsertedEvent(RuleMetadataUpsertedEvent eventData)
        {
            this.metadata = eventData.Metadata;

            // send responde back
            if (!IsRecovering && shardMetadataRequest != null)
            {
                Sender.Tell(shardMetadataRequest.GetResponse(), Self);
                shardMetadataRequest = null;
            }
        }

        private ParsedRule Parse(RuleMetadata metadata, InputRule rule)
        {
            String input = rule.PredicateExpression;
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new Predicate1Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Predicate1Parser parser = new Predicate1Parser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.pexpr();
            var visitor = new MyCustomVisitor2(metadata);
            var result = visitor.Visit(tree);
            result.RawRule = rule;
            return result;
        }
    }
}
