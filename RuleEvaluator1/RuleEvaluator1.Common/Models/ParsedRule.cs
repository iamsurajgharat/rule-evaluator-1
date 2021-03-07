using RuleEvaluator1.Common.Constants;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RuleEvaluator1.Common.Models
{
    public class ParsedRule
    {
        public readonly static HashSet<Tuple<RuleDataType, RuleOperator, RuleDataType>> SupportedBinaryRuleFormats;
        public InputRule RawRule { get; set; }
        public RuleOperator Operator { get; set; }
        public string Function { get; set; }
        public List<ParsedRule> Operands { get; set; }
        public RuleDataType ResultType { get; set; }
        public RuleType Type { get; set; }
        public object Value { get; set; }
        public string Variable { get; set; }

        static ParsedRule()
        {
            // all supported binary operator formats
            SupportedBinaryRuleFormats = new HashSet<Tuple<RuleDataType, RuleOperator, RuleDataType>>();

            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Plus, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Minus, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Multiply, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Division, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Modulo, RuleDataType.Number));

            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Gt, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Gt, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Gt, RuleDataType.DateTime));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Gt, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Gt, RuleDataType.DateTime));

            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Gte, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Gte, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Gte, RuleDataType.DateTime));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Gte, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Gte, RuleDataType.DateTime));

            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Lt, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Lt, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Lt, RuleDataType.DateTime));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Lt, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Lt, RuleDataType.DateTime));

            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Number, RuleOperator.Lte, RuleDataType.Number));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Lte, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.Date, RuleOperator.Lte, RuleDataType.DateTime));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Lte, RuleDataType.Date));
            SupportedBinaryRuleFormats.Add(GetTupleForBinaryOperatorFormat(RuleDataType.DateTime, RuleOperator.Lte, RuleDataType.DateTime));
        }

        private static Tuple<RuleDataType, RuleOperator, RuleDataType> GetTupleForBinaryOperatorFormat(RuleDataType left, RuleOperator opr, RuleDataType right)
        {
            return new Tuple<RuleDataType, RuleOperator, RuleDataType>(left, opr, right);
        }

        private static bool IsValidBinaryOperatorFormat(RuleDataType left, RuleOperator opr, RuleDataType right)
        {
            return opr == RuleOperator.Eq || opr == RuleOperator.NotEquals || SupportedBinaryRuleFormats.Contains(GetTupleForBinaryOperatorFormat(left, opr, right));
        }

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
            if (left == null || right == null || string.IsNullOrWhiteSpace(opr))
            {
                throw new RuleEvaluatorException(string.Format(Messages.EmptyRequest, $"Binary ParsedRule({left}|{opr}|{right})"));
            }

            // get operator as enum
            RuleOperator ruleOperator = opr.ToRuleOperator();

            if (!IsValidBinaryOperatorFormat(left.ResultType, ruleOperator, right.ResultType))
            {
                throw new RuleEvaluatorException(string.Format(Messages.IncompatibleArgumentTypesForOperator, $"{left.ResultType},{right.ResultType}", opr));
            }

            var result = new ParsedRule
            {
                Type = RuleType.BinaryOperator,
                Operator = ruleOperator,
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

        private Type GetCSharpTypeForRuleDataType(RuleDataType ruleDataType)
        {
            return ruleDataType switch
            {
                RuleDataType.Bool => typeof(bool),
                RuleDataType.Number => typeof(decimal),
                RuleDataType.Text => typeof(string)
            };
        }

        private static RuleDataType GetRuleDataType(object value)
        {
            if (value == null)
            {
                return RuleDataType.Null;
            }

            var valueType = value.GetType();

            if (valueType == typeof(byte) || valueType == typeof(short) || valueType == typeof(int) || valueType == typeof(long) || valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(decimal))
            {
                return RuleDataType.Number;
            }
            else if (valueType == typeof(bool))
            {
                return RuleDataType.Bool;
            }
            else if (valueType == typeof(string))
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
