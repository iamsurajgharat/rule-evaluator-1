using Akka.Actor;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuleEvaluator1.Service.Implementations
{
    public class RuleEvaluationService : IRuleEvaluationService
    {
        private readonly IActorProviderService actorProviderService;

        // maintain list of actors

        public RuleEvaluationService(IActorProviderService actorProviderService)
        {
            this.actorProviderService = actorProviderService ?? throw new ArgumentNullException(nameof(actorProviderService));
        }

        public async Task<Dictionary<string, BaseAckResponse>> AddUpdateRulesAsync(IEnumerable<InputRule> rules)
        {
            if (rules == null)
            {
                return new Dictionary<string, BaseAckResponse>();
            }

            var msg = new SaveRulesRequest
            {
                Id = Guid.NewGuid().ToString(),
                Rules = rules.ToList()
            };

            SaveRulesResponse result = await actorProviderService.GetRuleManagerActor().Ask<SaveRulesResponse>(msg);

            return result.Result;
        }

        public void DeleteRules(IEnumerable<string> ids)
        {

            if (ids == null)
            {
                return;
            }

            foreach (var item in ids)
            {

            }
        }

        public async Task<List<string>[]> EvaluateAsync(List<Record> records)
        {
            var request = new EvaluateRulesRequest
            {
                Id = Guid.NewGuid().ToString(),
                Records = records
            };

            var result = await actorProviderService.GetRuleManagerActor().Ask<EvaluateRulesResponse>(request);

            return result.Result;
        }

        public IEnumerable<InputRule> GetAllRules()
        {
            return new List<InputRule>();
        }

        public IEnumerable<InputRule> GetRules(IEnumerable<string> ids)
        {
            return new List<InputRule>();
        }

        public async Task<Dictionary<string, List<string>>> GetRuleShardsAsync()
        {
            var request = new GetInputRuleRequest
            {
                Id = Guid.NewGuid().ToString(),
            };

            var result = await actorProviderService.GetRuleManagerActor().Ask<GetInputRuleResponse>(request);

            return result.Result.ToDictionary(x => x.Key.ToString(), y => y.Value.Select(x => x.Id).ToList());
        }

        public async Task PutMetadataAsync(Dictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                return;
            }

            RuleMetadata processedMetadata = new RuleMetadata();
            List<string> invalidRuleDataTypes = new List<string>();
            foreach (var item in metadata)
            {
                if(!processedMetadata.Upsert(item.Key, item.Value))
                {
                    invalidRuleDataTypes.Add(item.Value);
                }
            }

            if (invalidRuleDataTypes.Count > 0)
            {
                throw new RuleEvaluatorException(string.Format(Common.Constants.Messages.InvalidRuleDataTypes, string.Join(',', invalidRuleDataTypes), string.Join(',', Enum.GetValues(typeof(RuleDataType)))));
            }


            BaseAckResponse response = await actorProviderService.GetRuleManagerActor().Ask<BaseAckResponse>(new SaveMetadataRequest { Id = Guid.NewGuid().ToString(), Metadata = processedMetadata });

            if (!response.IsSuccess)
            {
                throw new RuleEvaluatorException(response.Message);
            }
        }
    }
}
