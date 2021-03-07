using FluentAssertions;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Helpers;
using Xunit;

namespace RuleEvaluator1.Common.Test.Helpers
{
    public class ExtensionMethodsTests
    {
        public class ToRuleOperator : ExtensionMethodsTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("   ")]
            public void Should_throw_exception_for_null_or_empty_operator_string(string opr)
            {
                // act and assure
                opr.Invoking(x => x.ToRuleOperator()).Should().Throw<RuleEvaluatorException>();
            }

            [Theory]
            [InlineData("invalidOpr")]
            public void Should_throw_exception_for_invalid_operator_string(string opr)
            {
                // act and assure
                opr.Invoking(x => x.ToRuleOperator()).Should().Throw<RuleEvaluatorException>();
            }

            [Theory]
            [InlineData("+", RuleOperator.Plus)]
            [InlineData("-", RuleOperator.Minus)]
            [InlineData("*", RuleOperator.Multiply)]
            [InlineData("/", RuleOperator.Division)]
            [InlineData("%", RuleOperator.Modulo)]
            [InlineData(">", RuleOperator.Gt)]
            [InlineData(">=", RuleOperator.Gte)]
            [InlineData("<", RuleOperator.Lt)]
            [InlineData("<=", RuleOperator.Lte)]
            [InlineData("==", RuleOperator.Eq)]
            [InlineData("!=", RuleOperator.NotEquals)]
            public void Should_return_correct_enum_operator_value(string opr, RuleOperator expectedResult)
            {
                // ac
                var result = opr.ToRuleOperator();

                // act
                result.Should().Be(expectedResult);
            }
        }
    }
}
