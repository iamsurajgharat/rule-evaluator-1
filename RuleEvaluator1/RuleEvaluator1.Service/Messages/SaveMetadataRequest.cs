using RuleEvaluator1.Common.Models;
using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveMetadataRequest
    {
        public Dictionary<string, RuleDataType> Metadata { get; set; }
    }
}
