using System.Collections.Generic;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveRulesResponse : BaseRequest
    {
        public Dictionary<string,BaseAckResponse> Result { get; set; }
    }
}
