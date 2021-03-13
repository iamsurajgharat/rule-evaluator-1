using FluentValidation;
using RuleEvaluator1.Web.Models;

namespace RuleEvaluator1.Web.Validators
{
    public class RuleValidator : AbstractValidator<Rule>
    {
        public RuleValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.PredicateExpression).NotNull().NotEmpty();
            RuleFor(x => x.Result).NotNull();
        }
    }
}
