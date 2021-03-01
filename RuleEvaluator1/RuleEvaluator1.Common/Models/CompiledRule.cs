using System;
using System.Collections.Generic;

namespace RuleEvaluator1.Common.Models
{
    public class CompiledRule
    {
        public string Id { get; set; }
        public object Result { get; set; }
        public Delegate Rule { get; set; }
        public List<CompiledRuleParameter> Parameters { get; set; } = new List<CompiledRuleParameter>();

        public bool Evaluate(Record data)
        {
            if (Parameters.Count == 0)
            {
                return (bool)Rule.DynamicInvoke();
            }
            else
            {
                var arguments = GetParameterValues(data);
                return (bool)Rule.DynamicInvoke(arguments);
            }
        }

        public object EvaluateForResult(Record data)
        {
            return Evaluate(data) ? Result : null;
        }

        private object[] GetParameterValues(Record data)
        {
            var values = new object[Parameters.Count];

            if (data != null)
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    values[i] = data.Get(Parameters[i].Name, Parameters[i].Type);
                }
            }

            return values;
        }
    }
}
