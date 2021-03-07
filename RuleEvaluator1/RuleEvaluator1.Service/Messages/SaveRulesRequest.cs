using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveRulesRequest : BaseRequest
    {
        public List<InputRule> Rules { get; set; }
    }
}
