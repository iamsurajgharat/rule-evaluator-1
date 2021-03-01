using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class EvaluateRulesResponse : BaseRequest
    {
        public List<object>[] Result { get; set; }
    }
}
