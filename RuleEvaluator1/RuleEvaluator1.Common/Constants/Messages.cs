namespace RuleEvaluator1.Common.Constants
{
    public static class Messages
    {
        public static readonly string EmptyRequest = "Empty request body";

        public static readonly string RequiredDataMissing = "Required field/data is missing. [{0}]";

        public static readonly string InvalidRuleDataTypes = "These input data type are not supported : [{0}].\nValid values are {1}";

        public static readonly string SucessAck = "You request accepted successfully";

        public static readonly string IncompatibleArgumentTypesForOperator = "One or more of given arguments types [{0}] are not supported by the given operator [{1}]";

    }
}
