namespace RuleEvaluator1.Service.Messages
{
    public class BaseAckResponse
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public object Result { get; set; }
    }
}
