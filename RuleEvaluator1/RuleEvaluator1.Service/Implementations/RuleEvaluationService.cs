using Akka.Actor;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
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

        public void AddUpdateRules(IEnumerable<InputRule> rules)
        {
            if (rules == null)
            {
                return;
            }

            var msg = new SaveRuleRequest
            {
                Rules = rules.ToList()
            };
            actorProviderService.GetRuleManagerActor().Tell(msg);
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

        public async Task<List<object>[]> EvaluateAsync(List<Record> records)
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

        public async Task PutMetadataAsync(Dictionary<string, string> metadata)
        {
            if (metadata == null)
            {
                return;
            }

            Dictionary<string, RuleDataType> processedMetadata = new Dictionary<string, RuleDataType>();
            List<string> invalidRuleDataTypes = new List<string>();
            foreach (var item in metadata)
            {
                var convertedEnum = GetRuleDataType(item.Value);
                if (convertedEnum.HasValue)
                {
                    processedMetadata[item.Key] = convertedEnum.Value;
                }
                else
                {
                    invalidRuleDataTypes.Add(item.Value);
                }
            }

            if (invalidRuleDataTypes.Count > 0)
            {
                throw new RuleEvaluatorException(string.Format(Common.Constants.Messages.InvalidRuleDataTypes, string.Join(',', invalidRuleDataTypes), string.Join(',', Enum.GetValues(typeof(RuleDataType)))));
            }
            else
            {
                BaseAckResponse response = await actorProviderService.GetRuleManagerActor().Ask<BaseAckResponse>(new SaveMetadataRequest { Metadata = processedMetadata });

                if (!response.IsSuccess)
                {
                    throw new RuleEvaluatorException(response.Message);
                }
            }
        }

        private RuleDataType? GetRuleDataType(string ruleDataTypeAsString)
        {
            return Enum.TryParse(ruleDataTypeAsString, true, out RuleDataType result) ? (RuleDataType?)result : null;
        }
    }
}
