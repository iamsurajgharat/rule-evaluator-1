namespace RuleEvaluator1.Common.Enums
{
    public enum RuleDataType
    {
        Bool,
        Number,
        Text,
        Date,
        DateTime,
        Null
    }

    public enum RuleType
    {
        Constant,
        Variable,
        BinaryOperator,
        Function
    }

    public enum RuleOperator
    {
        Plus,
        Minus,
        Multiply,
        Division,
        UnaryMinus,
        Modulo,
        Gt,
        Lt,
        Gte,
        Lte,
        Eq,
        NotEquals,
        And,
        Or
    }
}
