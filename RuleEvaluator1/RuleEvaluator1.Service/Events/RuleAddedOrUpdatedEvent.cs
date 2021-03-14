using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Events
{
    public class RuleAddedOrUpdatedEvent
    {
        public List<ParsedRule> ParsedRules { get; set; }
    }
}
