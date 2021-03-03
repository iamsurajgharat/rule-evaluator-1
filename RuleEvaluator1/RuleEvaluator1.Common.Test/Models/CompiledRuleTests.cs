using FluentAssertions;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Helpers;
using RuleEvaluator1.Common.Models;
using System;
using System.Linq.Expressions;
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


            [Theory]
            [InlineData(100, true)]
            [InlineData(200, false)]
            public void Should_evaluate_predicate_with_arguments_correctly(decimal field2Value, bool expectedResult)
            {
                // arrange
                InputRule rawRule = new InputRule();
                var parameters = Utility.List(Expression.Parameter(typeof(decimal), "field1"), Expression.Parameter(typeof(decimal), "field2"));
                var subject = new CompiledRule(rawRule, (Func<decimal,decimal,bool>)((x,y) => x==y), parameters);

                // input record
                var record = new Common.Models.Record();
                record.Set("field1", 100m);
                record.Set("field2", field2Value);

                // act
                var result = subject.Evaluate(record);

                // assure
                result.Should().Be(expectedResult);
            }

            [Fact]
            public void Should_evaluate_non_predicate_rule_correctly()
            {
                // arrange
                InputRule rawRule = new InputRule();
                var subject = new CompiledRule(rawRule, (Func<int>)(() => 100), null);

                // act
                var result = subject.Evaluate(new Common.Models.Record());

                // assure
                result.Should().Be(100);
            }
        }

        public class EvaluateForResult : CompiledRuleTests
        {
            [Fact]
            public void Should_return_null_if_eval_gives_false()
            {
                // arrange
                InputRule rawRule = new InputRule { Result = "rule-that-matched" };
                var subject = new CompiledRule(rawRule, (Func<bool>)(() => false), null);

                // act
                var result = subject.EvaluateForResult(new Common.Models.Record());

                // assure
                result.Should().BeNull();
            }

            [Fact]
            public void Should_return_result_if_eval_gives_true()
            {
                // arrange
                InputRule rawRule = new InputRule { Result = "rule-that-matched" };
                var subject = new CompiledRule(rawRule, (Func<bool>)(() => true), null);

                // act
                var result = subject.EvaluateForResult(new Common.Models.Record());

                // assure
                result.Should().Be("rule-that-matched");
            }

            [Theory]
            [InlineData("true", "rule-that-matched")]
            [InlineData("TRUE", "rule-that-matched")]
            [InlineData("trUE", "rule-that-matched")]
            [InlineData("false", null)]
            [InlineData("FALSE", null)]
            public void Should_return_result_if_eval_gives_boolean_value_in_string_format(string retValue, string expectedResult)
            {
                // arrange
                InputRule rawRule = new InputRule { Result = "rule-that-matched" };
                var subject = new CompiledRule(rawRule, (Func<string>)(() => retValue), null);

                // act
                var result = subject.EvaluateForResult(new Common.Models.Record());

                // assure
                result.Should().Be(expectedResult);
            }

            [Fact]
            public void Should_throw_exception_if_rule_is_not_predicate()
            {
                // arrange
                InputRule rawRule = new InputRule { Result = "rule-that-matched" };
                var subject = new CompiledRule(rawRule, (Func<string>)(() => "Yes and No!"), null);

                // act and assure
                subject.Invoking(x => x.EvaluateForResult(new Common.Models.Record())).Should().Throw<RuleEvaluatorException>();
            }

            

            [Fact]
            public void Should_throw_exception_if_rule_returns_null()
            {
                // arrange
                InputRule rawRule = new InputRule { Result = "rule-that-matched" };
                var subject = new CompiledRule(rawRule, (Func<string>)(() => null), null);

                // act and assure
                subject.Invoking(x => x.EvaluateForResult(new Common.Models.Record())).Should().Throw<RuleEvaluatorException>();
            }
        }
    }
}
