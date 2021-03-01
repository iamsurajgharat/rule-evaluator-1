using RuleEvaluator1.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RuleEvaluator1.Common.Models
{
    public class ParsedRule
    {
        public InputRule RawRule { get; set; }
        public RuleOperator Operator { get; set; }
        public string Function { get; set; }
        public List<ParsedRule> Operands { get; set; }
        public RuleDataType ResultType { get; set; }
        public RuleType Type { get; set; }
        public object Value { get; set; }
        public string Variable { get; set; }

        public static ParsedRule Parameter(string name)
        {
            return new ParsedRule
            {
                Type = RuleType.Variable,
                Variable = name
            };
        }

        public static ParsedRule Constant(object value)
        {
            return new ParsedRule
            {
                Type = RuleType.Constant,
                Value = value
            };
        }

        public static ParsedRule MakeBinary(ParsedRule left, string opr, ParsedRule right)
        {
            return new ParsedRule
            {
                Type = RuleType.BinaryOperator,
                Operator = ToMyExpressionOperator(opr),
                Operands = new List<ParsedRule>
                {
                    left,
                    right
                }
            };
        }

        public CompiledRule Compile(Dictionary<string, RuleDataType> metadata)
        {
            var paramsl = new List<ParameterExpression>();
            Expression cex = Compile(metadata, paramsl, typeof(bool));
            return new CompiledRule(RawRule, Expression.Lambda(cex, paramsl).Compile(), paramsl);
        }

        public Expression Compile(Dictionary<string, RuleDataType> metadata, List<ParameterExpression> parameters, Type resultType)
        {
            if (parameters == null)
            {
                parameters = new List<ParameterExpression>();
            }

            switch (Type)
            {
                case RuleType.BinaryOperator:
                    var operandType = GetOperandTypeForOperator(Operator);
                    return CompileBinaryOperator(Operands[0].Compile(metadata, parameters, operandType), Operator, Operands[1].Compile(metadata, parameters, operandType));

                case RuleType.Variable:
                    var result = Expression.Parameter(GetCSharpTypeForRuleDataType(metadata, Variable), Variable);
                    parameters.Add(result);
                    return result;
                case RuleType.Constant:
                    return Expression.Constant(Value, resultType);
            }

            return null;
        }

        private Expression CompileBinaryOperator(Expression left, RuleOperator opr, Expression right)
        {
            return opr switch
            {
                RuleOperator.Plus => Expression.MakeBinary(ExpressionType.Add, left, right),

                RuleOperator.Minus => Expression.MakeBinary(ExpressionType.Subtract, left, right),

                RuleOperator.Multiply => Expression.MakeBinary(ExpressionType.Multiply, left, right),

                RuleOperator.Division => Expression.MakeBinary(ExpressionType.Divide, left, right),

                RuleOperator.Gt => Expression.MakeBinary(ExpressionType.GreaterThan, left, right),

                RuleOperator.Gte => Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, left, right),

                RuleOperator.Lt => Expression.MakeBinary(ExpressionType.LessThan, left, right),

                RuleOperator.Lte => Expression.MakeBinary(ExpressionType.LessThanOrEqual, left, right),

                RuleOperator.Eq => Expression.MakeBinary(ExpressionType.Equal, left, right),

                RuleOperator.NotEquals => Expression.MakeBinary(ExpressionType.NotEqual, left, right),

                _ => throw new NotImplementedException("Operator"),
            };
        }

        private Type GetOperandTypeForOperator(RuleOperator @operator)
        {
            switch (@operator)
            {
                case RuleOperator.Plus:
                case RuleOperator.Minus:
                case RuleOperator.Multiply:
                case RuleOperator.Division:
                case RuleOperator.Gt:
                case RuleOperator.Gte:
                case RuleOperator.Lt:
                case RuleOperator.Lte:
                    return typeof(decimal);
                case RuleOperator.And:
                case RuleOperator.Or:
                    return typeof(bool);
                case RuleOperator.Eq:
                case RuleOperator.NotEquals:
                    return typeof(object);
            }

            return typeof(object);
        }

        private Type GetCSharpTypeForRuleDataType(Dictionary<string, RuleDataType> metadata, string fieldName)
        {
            if (metadata.TryGetValue(fieldName, out var ruleDataType))
            {
                return ruleDataType switch
                {
                    RuleDataType.Bool => typeof(bool),
                    RuleDataType.Number => typeof(decimal),
                    RuleDataType.Text => typeof(string)
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static RuleOperator ToMyExpressionOperator(string opr)
        {
            if (string.IsNullOrWhiteSpace(opr))
            {
                throw new Exception("Invalid operator : " + opr);
            }

            return opr switch
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
                _ => throw new Exception("Invalid operator : " + opr)
            };

        }
    }
}
