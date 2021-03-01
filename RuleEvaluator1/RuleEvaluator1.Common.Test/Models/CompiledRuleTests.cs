using FluentAssertions;
using RuleEvaluator1.Common.Models;
using System;
using Xunit;

namespace RuleEvaluator1.Common.Test.Models
{
    public class CompiledRuleTests
    {
        public class Evaluate : CompiledRuleTests
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_evaluate_zero_argument_predicate_correctly(bool value)
            {
                // arrange
                InputRule rawRule = new InputRule();
                var subject = new CompiledRule(rawRule, (Func<bool>)(() => value), null);

                // act
                var result = subject.Evaluate(new Common.Models.Record());

                // assure
                result.Should().Be(value);
            }
        }
    }
}
