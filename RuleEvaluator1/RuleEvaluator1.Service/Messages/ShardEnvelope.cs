namespace RuleEvaluator1.Service.Messages
{
    public class ShardEnvelope
    {
        public readonly string entityId;
        public readonly object message;

        public ShardEnvelope(string entityId, object message)
        {
            this.entityId = entityId;
            this.message = message;
        }
    }
}
