using FluentAssertions;
using RuleEvaluator1.Common.Constants;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using System;
using Xunit;

namespace RuleEvaluator1.Common.Test.Models
{
    public class ParsedRuleTests
    {
        public class Parameter : ParsedRuleTests
        {
            [Fact]
            public void Should_create_instance_for_parameter()
            {
                // act
                var result = ParsedRule.Parameter("fieldName1", RuleDataType.Number);

                // assure
                result.Should().NotBeNull();
                result.ResultType.Should().Be(RuleDataType.Number);
                result.Type.Should().Be(RuleType.Variable);
                result.Variable.Should().Be("fieldName1");
                result.Value.Should().BeNull();
            }
        }

        public class Constant : ParsedRuleTests
        {
            [Theory]
            [InlineData(true, RuleDataType.Bool)]
            [InlineData(false, RuleDataType.Bool)]
            [InlineData(10, RuleDataType.Number)]
            [InlineData(10.20, RuleDataType.Number)]
            [InlineData("Suraj", RuleDataType.Text)]
            public void Should_create_instance_for_constant_value(object value, RuleDataType dataType)
            {
                // act
                var result = ParsedRule.Constant(value);

                // assure
                result.Should().NotBeNull();
                result.ResultType.Should().Be(dataType);
                result.Type.Should().Be(RuleType.Constant);
                result.Variable.Should().BeNull();
                result.Value.Should().Be(value);
            }
        }

        public class MakeBinary : ParsedRuleTests
        {
            [Fact]
            public void Should_create_instance_for_binary()
            {
                // arrange
                var left = ParsedRule.Parameter("field1", RuleDataType.Number);
                var right = ParsedRule.Parameter("field2", RuleDataType.Number);

                // act
                ParsedRule result = ParsedRule.MakeBinary(left, "+", right);

                // assure
                result.Should().NotBeNull();
                result.Type.Should().Be(RuleType.BinaryOperator);
                result.Operator.Should().Be(RuleOperator.Plus);
                result.Operands.Should().HaveCount(2);
                result.Operands[0].Should().Be(left);
                result.Operands[1].Should().Be(right);
                result.ResultType.Should().Be(RuleDataType.Number);
                result.Variable.Should().BeNull();
                result.Value.Should().BeNull();
            }

            [Theory]
            [InlineData("+", RuleOperator.Plus, RuleDataType.Number)]
            [InlineData("-", RuleOperator.Minus, RuleDataType.Number)]
            [InlineData("*", RuleOperator.Multiply, RuleDataType.Number)]
            [InlineData("/", RuleOperator.Division, RuleDataType.Number)]
            [InlineData("%", RuleOperator.Modulo, RuleDataType.Number)]
            [InlineData(">", RuleOperator.Gt, RuleDataType.Bool)]
            [InlineData(">=", RuleOperator.Gte, RuleDataType.Bool)]
            [InlineData("<", RuleOperator.Lt, RuleDataType.Bool)]
            [InlineData("<=", RuleOperator.Lte, RuleDataType.Bool)]
            [InlineData("==", RuleOperator.Eq, RuleDataType.Bool)]
            [InlineData("!=", RuleOperator.NotEquals, RuleDataType.Bool)]
            public void Should_create_instance_for_binary_for_all_operators(string opr, RuleOperator expectedResultOpr, RuleDataType expectedResultType)
            {
                // arrange
                var left = ParsedRule.Parameter("field1", RuleDataType.Number);
                var right = ParsedRule.Parameter("field2", RuleDataType.Number);

                // act
                var result = ParsedRule.MakeBinary(left, opr, right);

                // assure
                result.Should().NotBeNull();
                result.Type.Should().Be(RuleType.BinaryOperator);
                result.Operator.Should().Be(expectedResultOpr);
                result.Operands.Should().HaveCount(2);
                result.Operands[0].Should().Be(left);
                result.Operands[1].Should().Be(right);
                result.ResultType.Should().Be(expectedResultType);
                result.Variable.Should().BeNull();
                result.Value.Should().BeNull();
            }

            // TODO : Make it more exhaustive
            [Theory]
            [InlineData(RuleDataType.Number, "+", RuleDataType.Bool)]
            [InlineData(RuleDataType.Bool, "-", RuleDataType.Bool)]
            [InlineData(RuleDataType.Bool, "*", RuleDataType.Number)]
            [InlineData(RuleDataType.Text, "/", RuleDataType.Number)]
            [InlineData(RuleDataType.Text, "%", RuleDataType.Date)]
            [InlineData(RuleDataType.Text, ">", RuleDataType.Date)]
            public void Should_throw_exception_for_invalid_argument_types(RuleDataType leftType, string opr, RuleDataType rightType)
            {
                // arrange
                var left = ParsedRule.Parameter("field1", leftType);
                var right = ParsedRule.Parameter("field2", rightType);

                // act and assure
                Action action = () => ParsedRule.MakeBinary(left, opr, right);
                action.Invoking(x => x()).Should().Throw<RuleEvaluatorException>().WithMessage(string.Format(Messages.IncompatibleArgumentTypesForOperator, $"{leftType},{rightType}", opr));

            }

            [Fact]
            public void ttt()
            {
                var t1 = new Tuple<RuleOperator, RuleDataType>(RuleOperator.Plus, RuleDataType.Number);
                var t2 = new Tuple<RuleOperator, RuleDataType>(RuleOperator.Plus, RuleDataType.Text);

                DateTime d1 = new DateTime(2020, 1, 15).Date;
                DateTime d2 = new DateTime(2020, 1, 15, 10,30,20);

                (d1 < d2).Should().BeTrue();
            }
        }
    }
}
