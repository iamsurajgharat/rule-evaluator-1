using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEvaluator1.Service.Helpers
{
    public static class CommonUtil
    {
        public static int GetRuleActorIdSequence(string ruleId, int numberOfShards)
        {
            return string.IsNullOrWhiteSpace(ruleId) ? 0 : (Math.Abs(ruleId.GetHashCode()) % numberOfShards);
        }
    }
}
