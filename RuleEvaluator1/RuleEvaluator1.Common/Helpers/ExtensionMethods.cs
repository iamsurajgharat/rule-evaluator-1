using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using System;

namespace RuleEvaluator1.Common.Helpers
{
    public static class ExtensionMethods
    {
        public static RuleOperator ToRuleOperator(this string receiver)
        {
            if (string.IsNullOrWhiteSpace(receiver))
            {
                throw new RuleEvaluatorException("Invalid operator : " + receiver);
            }

            return receiver switch
            {
                "+" => RuleOperator.Plus,
                "-" => RuleOperator.Minus,
                "*" => RuleOperator.Multiply,
                "/" => RuleOperator.Division,
                "%" => RuleOperator.Modulo,
                ">" => RuleOperator.Gt,
                ">=" => RuleOperator.Gte,
                "<" => RuleOperator.Lt,
                "<=" => RuleOperator.Lte,
                "==" => RuleOperator.Eq,
                "!=" => RuleOperator.NotEquals,
                _ => throw new RuleEvaluatorException("Invalid operator : " + receiver)
            };
        }
    }
}
