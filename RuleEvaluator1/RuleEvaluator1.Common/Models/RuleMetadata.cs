using RuleEvaluator1.Common.Enums;
using System;
using System.Collections.Generic;

namespace RuleEvaluator1.Common.Models
{
    public class RuleMetadata
    {
        public Dictionary<string, RuleDataType> Values { get; set; }

        public RuleMetadata()
        {
            Values = new Dictionary<string, RuleDataType>();
        }

        public RuleMetadata(Dictionary<string, RuleDataType> values) : this()
        {
            foreach (var item in values)
            {
                this.Values[item.Key] = item.Value;
            }
        }

        public bool IsEmpty() => Values.Count == 0;

        public RuleDataType? GetRuleDataType(string fieldName) => Values.TryGetValue(fieldName, out var result) ? (RuleDataType?)result : null;

        public bool Upsert(string fieldName, string dataType)
        {
            var convertedEnum = ConvertToEnum(dataType);
            if (!convertedEnum.HasValue)
            {
                return false;
            }

            this.Values[fieldName] = convertedEnum.Value;

            return true;
        }

        private RuleDataType? ConvertToEnum(string ruleDataTypeAsString)
        {
            return Enum.TryParse(ruleDataTypeAsString, true, out RuleDataType result) ? (RuleDataType?)result : null;
        }
    }
}
