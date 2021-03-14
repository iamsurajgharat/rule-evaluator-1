using RuleEvaluator1.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RuleEvaluator1.Common.Models
{
    public class CompiledRule
    {
        private readonly InputRule rawRule;
        private readonly Delegate rule;
        private readonly List<CompiledRuleParameter> parameters = new List<CompiledRuleParameter>();

        public InputRule RawRule
        {
            get
            {
                return rawRule;
            }
        }

        public CompiledRule(InputRule rule, Delegate compiledExpression, List<ParameterExpression> parameters)
        {
            this.rawRule = rule ?? throw new ArgumentNullException(nameof(rule));
            this.rule = compiledExpression ?? throw new ArgumentNullException(nameof(compiledExpression));
            this.parameters = (parameters ?? Enumerable.Empty<ParameterExpression>()).Select(x => new CompiledRuleParameter { Name = x.Name, Type = x.Type }).ToList();
        }

        public object Evaluate(Record data)
        {
            if (parameters.Count == 0)
            {
                return rule.DynamicInvoke();
            }
            else
            {
                object[] arguments = GetParameterValues(data);
                return rule.DynamicInvoke(arguments);
            }
        }

        public string EvaluateForResult(Record data)
        {
            var result = Evaluate(data);

            if (result == null || !bool.TryParse(result.ToString(), out var value))
            {
                throw new RuleEvaluatorException("Rule does not return valid boolean value [" + result + "]");
            }
            else if (value)
            {
                return rawRule.Result;
            }

            return null;
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
