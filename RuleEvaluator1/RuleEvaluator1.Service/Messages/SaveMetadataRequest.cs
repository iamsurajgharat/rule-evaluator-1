using RuleEvaluator1.Common.Models;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveMetadataRequest
    {
        public RuleMetadata Metadata { get; set; }
    }
}
