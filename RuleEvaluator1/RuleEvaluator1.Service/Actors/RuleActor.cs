using Akka.Persistence;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Actors
{
    public class RuleActor : ReceivePersistentActor
    {
        private readonly Dictionary<string,ParsedRule> rules;
        private readonly Dictionary<string, CompiledRule> compiledRules;
        public override string PersistenceId => $"{Context.Parent.Path.Name}/{Context.Self.Path.Name}";

        public RuleActor()
        {
            rules = new Dictionary<string, ParsedRule>();
            compiledRules = new Dictionary<string, CompiledRule>();

            // process add/update rules request
            Command<SaveParsedRulesRequest>(ProcessSaveParsedRuleRequest);

            // evaluate
            Command<EvaluateShardRulesRequest>(ProcessEvaluateShardRulesRequest);

            Command<object>(x =>
            {
                Context.System.Log.Warning("Unhandled message :" + x);
            });
        }

        private void ProcessEvaluateShardRulesRequest(EvaluateShardRulesRequest request)
        {
            var response = request.GetResponse();
            foreach (var rule in compiledRules)
            {
                for (int i = 0; i < request.Records.Count; i++)
                {
                    var result = rule.Value.EvaluateForResult(request.Records[i]);
                    if (result != null)
                    {
                        response.Result[i].Add(result);
                    }
                }
            }

            // send responde back
            Sender.Tell(response, Self);
        }

        private void ProcessSaveParsedRuleRequest(SaveParsedRulesRequest request)
        {
            foreach (var item in request.Rules)
            {
                rules[item.RawRule.Id] = item;
                compiledRules[item.RawRule.Id] = item.Compile(request.Metadata);
            }
        }
    }
}
