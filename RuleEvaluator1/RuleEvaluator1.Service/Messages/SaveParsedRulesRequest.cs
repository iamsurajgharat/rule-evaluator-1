using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveParsedRulesRequest
    {
        public RuleMetadata Metadata { get; set; }
        public List<ParsedRule> Rules { get; set; }
    }
}
