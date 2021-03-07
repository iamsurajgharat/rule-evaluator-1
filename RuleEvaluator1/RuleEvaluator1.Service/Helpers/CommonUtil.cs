using System;

namespace RuleEvaluator1.Service.Helpers
{
    public static class CommonUtil
    {
        public static int GetRuleActorIdSequence(string ruleId, int numberOfShards)
        {
            return string.IsNullOrWhiteSpace(ruleId) ? 0 : GetRuleActorIdSequence(Math.Abs(ruleId.GetHashCode()), numberOfShards);
        }

        public static int GetRuleActorIdSequence(int code, int numberOfShards)
        {
            return code % numberOfShards;
        }
    }
}
