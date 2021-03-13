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

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
