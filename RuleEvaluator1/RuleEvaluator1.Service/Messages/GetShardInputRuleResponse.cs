using RuleEvaluator1.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEvaluator1.Service.Messages
{
    public class GetShardInputRuleResponse : ShardBaseRequest
    {
        public List<InputRule> Rules { get; set; }
    }
}
