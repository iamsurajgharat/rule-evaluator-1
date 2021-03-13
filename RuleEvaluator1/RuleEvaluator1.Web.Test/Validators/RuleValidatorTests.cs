using FluentValidation.TestHelper;
using RuleEvaluator1.Web.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace RuleEvaluator1.Web.Test.Validators
{
    public class RuleValidatorTests
    {
        protected RuleValidator subject;

        public RuleValidatorTests()
        {
            subject = new RuleValidator();
        }

        public class Rule : RuleValidatorTests
        {
            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("  ")]
            public void Should_have_validation_error_for_null_id(string id)
            {
                var model = new Web.Models.Rule { PredicateExpression = "A > 20", Result = "sadad", Id = id };
                var result = subject.TestValidate(model);
                result.ShouldHaveValidationErrorFor(rule => rule.Id);
            }
        }
    }
}
