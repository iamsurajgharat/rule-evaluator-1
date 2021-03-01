using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveRuleRequest
    {
        public List<InputRule> Rules { get; set; }
    }
}
