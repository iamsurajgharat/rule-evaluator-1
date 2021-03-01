namespace RuleEvaluator1.Web.Models
{
    public class WebApiResponse
    {
        public string AckMessage { get; set; }
        public object AdditionalDetails { get; set; }
        public bool IsSuccess { get; set; }
        public string RequestId { get; set; }
        public object Result { get; set; }

        public readonly static WebApiResponse EmptyInputInstance = new WebApiResponse
        {
            AckMessage = Common.Constants.Messages.SucessAck,
            IsSuccess = true
        };

        public readonly static WebApiResponse SuccessInstance = new WebApiResponse
        {
            AckMessage = Common.Constants.Messages.SucessAck,
            IsSuccess = true
        };
    }
}
