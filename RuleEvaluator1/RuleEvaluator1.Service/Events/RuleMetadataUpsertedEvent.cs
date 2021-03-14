using RuleEvaluator1.Common.Models;

namespace RuleEvaluator1.Service.Events
{
    public class RuleMetadataUpsertedEvent
    {
        public RuleMetadata Metadata { get; set; }
    }
}
