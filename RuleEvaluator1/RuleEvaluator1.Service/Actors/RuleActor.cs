using Akka.Persistence;
using Antlr4.Runtime;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Parser;
using RuleEvaluator1.Service.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleActor : ReceivePersistentActor
    {
        private readonly Dictionary<string, ParsedRule> rules;
        private readonly Dictionary<string, CompiledRule> compiledRules;
        public override string PersistenceId => $"{Context.Parent.Path.Name}/{Context.Self.Path.Name}";

        public RuleActor()
        {
            rules = new Dictionary<string, ParsedRule>();
            compiledRules = new Dictionary<string, CompiledRule>();

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
        }

        private void ProcessGetInputRulesRequest(GetShardInputRuleRequest request)
        {
            Sender.Tell(request.CreateResponse(rules.Values.Select(x => x.RawRule)), Self);
        }

        private void ProcessEvaluateShardRulesRequest(EvaluateShardRulesRequest request)
        {
            EvaluateShardRulesResponse response = request.GetResponse();
            foreach (var rule in compiledRules)
            {
                for (int i = 0; i < request.Records.Count; i++)
                {
                    object result = rule.Value.EvaluateForResult(request.Records[i]);
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
            foreach (var item in request.Rules)
            {
                try
                {
                    ParsedRule parsedRule = Parse(request.Metadata, item);
                    rules[item.Id] = parsedRule;
                    compiledRules[item.Id] = parsedRule.Compile();
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

            // send ack to the manager
            Sender.Tell(response, Self);
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
