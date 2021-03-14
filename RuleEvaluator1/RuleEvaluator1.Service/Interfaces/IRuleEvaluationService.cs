using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RuleEvaluator1.Service.Interfaces
{
    public interface IRuleEvaluationService
    {
        Task<Dictionary<string,BaseAckResponse>> AddUpdateRulesAsync(IEnumerable<InputRule> rules);
        IEnumerable<InputRule> GetRules(IEnumerable<string> ids);
        IEnumerable<InputRule> GetAllRules();
        Task<Dictionary<string, List<string>>> GetRuleShardsAsync();
        void DeleteRules(IEnumerable<string> ids);
        Task<List<string>[]> EvaluateAsync(List<Record> records);
        Task PutMetadataAsync(Dictionary<string, string> metadata);
    }
}
