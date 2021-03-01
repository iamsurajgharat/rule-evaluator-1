using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RuleEvaluator1.Common.Models
{
    public class CompiledRule
    {
        private readonly InputRule rawRule;
        private readonly Delegate predicate;
        private readonly List<CompiledRuleParameter> parameters = new List<CompiledRuleParameter>();

        public CompiledRule(InputRule rule, Delegate compiledExpression, List<ParameterExpression> parameters)
        {
            this.rawRule = rule ?? throw new ArgumentNullException(nameof(rule));
            this.predicate = compiledExpression ?? throw new ArgumentNullException(nameof(compiledExpression));
            this.parameters = (parameters ?? Enumerable.Empty<ParameterExpression>()).Select(x => new CompiledRuleParameter { Name = x.Name, Type = x.Type }).ToList();
        }

        public bool Evaluate(Record data)
        {
            if (parameters.Count == 0)
            {
                return (bool)predicate.DynamicInvoke();
            }
            else
            {
                object[] arguments = GetParameterValues(data);
                return (bool)predicate.DynamicInvoke(arguments);
            }
        }

        public object EvaluateForResult(Record data)
        {
            return Evaluate(data) ? rawRule.Result : null;
        }

        private object[] GetParameterValues(Record data)
        {
            var values = new object[parameters.Count];

            if (data != null)
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    values[i] = data.Get(parameters[i].Name, parameters[i].Type);
                }
            }

            return values;
        }
    }
}
