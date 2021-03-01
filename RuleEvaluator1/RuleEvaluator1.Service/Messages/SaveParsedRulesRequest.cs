using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveParsedRulesRequest
    {
        public Dictionary<string,RuleDataType> Metadata { get; set; }
        public List<ParsedRule> Rules { get; set; }
    }
}
