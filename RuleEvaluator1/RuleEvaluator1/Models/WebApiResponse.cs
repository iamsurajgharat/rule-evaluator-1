using RuleEvaluator1.Service.Messages;
using System.Collections.Generic;

namespace RuleEvaluator1.Web.Models
{
    public class WebApiResponse
    {
        private bool isBadRequest;

        public string AckMessage { get; set; }
        public object AdditionalDetails { get; set; }
        public bool IsSuccess { get; set; }
        public string RequestId { get; set; }
        public object Result { get; set; }

        public readonly static WebApiResponse EmptyInputInstance = new WebApiResponse
        {
            AckMessage = Common.Constants.Messages.EmptyRequest,
            isBadRequest = true,
            IsSuccess = false
        };

        public readonly static WebApiResponse SuccessInstance = new WebApiResponse
        {
            AckMessage = Common.Constants.Messages.SucessAck,
            IsSuccess = true
        };

        public static explicit operator WebApiResponse(Dictionary<string, BaseAckResponse> response)
        {
            var result = new WebApiResponse
            {
                Result = response,
                IsSuccess = true,
                isBadRequest = false
            };

            if (response == null || response.Count == 0)
            {
                return result;
            }

            result.isBadRequest = true;

            foreach (var item in response)
            {
                if (item.Value.IsSuccess)
                {
                    result.isBadRequest = false;
                }
                else
                {
                    result.IsSuccess = false;
                }

                if (!result.IsSuccess && !result.isBadRequest)
                {
                    break;
                }
            }

            return result;
        }

        public bool IsBadRequest() => this.isBadRequest;
    }
}
