namespace RuleEvaluator1.Web.Models
{
    public class Rule
    {
        public string Id { get; set; }
        public string PredicateExpression { get; set; }
        public object Result { get; set; }
    }
}
