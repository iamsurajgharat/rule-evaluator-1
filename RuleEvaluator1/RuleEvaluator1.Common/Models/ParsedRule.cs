using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
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

        public static ParsedRule Parameter(string name, RuleDataType dataType)
        {
            return new ParsedRule
            {
                Type = RuleType.Variable,
                ResultType = dataType,
                Variable = name
            };
        }

        public static ParsedRule Constant(object value)
        {
            return new ParsedRule
            {
                Type = RuleType.Constant,
                Value = value,
                ResultType = GetRuleDataType(value)
            };
        }

        public static ParsedRule MakeBinary(ParsedRule left, string opr, ParsedRule right)
        {
            var result = new ParsedRule
            {
                Type = RuleType.BinaryOperator,
                Operator = ToMyExpressionOperator(opr),
                Operands = new List<ParsedRule>
                {
                    left,
                    right
                }
            };

            result.ResultType = GetRuleDataType(result.Operator);

            return result;
        }

        public CompiledRule Compile()
        {
            var paramsl = new List<ParameterExpression>();
            Expression cex = Compile(paramsl);
            return new CompiledRule(RawRule, Expression.Lambda(Reduce(cex), paramsl).Compile(), paramsl);
        }

        private Expression Compile(List<ParameterExpression> parameters)
        {
            if (parameters == null)
            {
                parameters = new List<ParameterExpression>();
            }

            switch (Type)
            {
                case RuleType.BinaryOperator:
                    return CompileBinaryOperator(Operands[0].Compile(parameters), Operator, Operands[1].Compile(parameters));

                case RuleType.Variable:
                    var result = Expression.Parameter(GetCSharpTypeForRuleDataType(ResultType), Variable);
                    parameters.Add(result);
                    return result;
                case RuleType.Constant:
                    return Expression.Constant(Value, GetCSharpTypeForRuleDataType(ResultType));
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

        private Type GetCSharpTypeForRuleDataType(RuleDataType ruleDataType)
        {
            return ruleDataType switch
            {
                RuleDataType.Bool => typeof(bool),
                RuleDataType.Number => typeof(decimal),
                RuleDataType.Text => typeof(string)
            };
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

        private static RuleDataType GetRuleDataType(object value)
        {
            if (value == null)
            {
                return RuleDataType.Null;
            }

            var valueType = value.GetType();

            if(valueType == typeof(byte) || valueType == typeof(short) || valueType == typeof(int) || valueType == typeof(long) || valueType == typeof(double) || valueType == typeof(float)|| valueType == typeof(decimal))
            {
                return RuleDataType.Number;
            }
            else if(valueType == typeof(bool))
            {
                return RuleDataType.Bool;
            }
            else if(valueType == typeof(string))
            {
                return RuleDataType.Text;
            }

            throw new RuleEvaluatorException("Unsupported value");
        }

        private static RuleDataType GetRuleDataType(RuleOperator value)
        {
            return value switch
            {
                RuleOperator.And => RuleDataType.Bool,
                RuleOperator.Division => RuleDataType.Number,
                RuleOperator.Eq => RuleDataType.Bool,
                RuleOperator.Gt => RuleDataType.Bool,
                RuleOperator.Gte => RuleDataType.Bool,
                RuleOperator.Lt => RuleDataType.Bool,
                RuleOperator.Lte => RuleDataType.Bool,
                RuleOperator.Minus => RuleDataType.Number,
                RuleOperator.Modulo => RuleDataType.Number,
                RuleOperator.Multiply => RuleDataType.Number,
                RuleOperator.NotEquals => RuleDataType.Bool,
                RuleOperator.Or => RuleDataType.Bool,
                RuleOperator.Plus => RuleDataType.Number,
                RuleOperator.UnaryMinus => RuleDataType.Number
            };
        }

        private Expression Reduce(Expression exp)
        {
            if (exp.CanReduce)
            {
                return Reduce(exp.Reduce());
            }

            return exp;
        }
    }
}
