using System;
using System.Collections.Generic;
using System.Text;

namespace RuleEvaluator1.Service.Messages
{
    public class SaveShardRulesResponse : ShardBaseRequest
    {
        public Dictionary<string, BaseAckResponse> Result { get; set; }
    }
}
