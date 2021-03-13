using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEvaluator1.Service.Messages
{
    public class GetInputRuleResponse : BaseRequest
    {
        public Dictionary<RuleShard, List<InputRule>> Result { get; set; }
    }
}
