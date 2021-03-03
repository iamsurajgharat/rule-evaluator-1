using FluentAssertions;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Helpers;
using RuleEvaluator1.Common.Models;
using System;
using System.Linq.Expressions;
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
    }
}
