﻿using RuleEvaluator1.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RuleEvaluator1.Service.Interfaces
{
    public interface IRuleEvaluationService
    {
        void AddUpdateRules(IEnumerable<InputRule> rules);
        IEnumerable<InputRule> GetRules(IEnumerable<string> ids);
        IEnumerable<InputRule> GetAllRules();
        void DeleteRules(IEnumerable<string> ids);
        Task<List<object>[]> EvaluateAsync(List<Record> records);
        Task PutMetadataAsync(Dictionary<string, string> metadata);
    }
}
