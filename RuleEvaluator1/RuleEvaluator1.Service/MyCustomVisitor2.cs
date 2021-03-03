using Antlr4.Runtime.Misc;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Parser;

namespace RuleEvaluator1.Service
{
    public class MyCustomVisitor2 : Predicate1BaseVisitor<ParsedRule>
    {
        private readonly RuleMetadata metadata;

        public MyCustomVisitor2()
        {
            metadata = new RuleMetadata();
        }

        public MyCustomVisitor2(RuleMetadata metadata)
        {
            this.metadata = metadata;
        }

        public override ParsedRule VisitPexpr_Poperator([NotNull] Predicate1Parser.Pexpr_PoperatorContext context)
        {
            return ParsedRule.MakeBinary(Visit(context.GetChild(0)), context.POPERATORS().GetText(), Visit(context.GetChild(2)));
        }

        public override ParsedRule VisitExpr_BinaryOperation([NotNull] Predicate1Parser.Expr_BinaryOperationContext context)
        {
            return ParsedRule.MakeBinary(Visit(context.GetChild(0)), context.OPERATOR().GetText(), Visit(context.GetChild(2)));
        }

        public override ParsedRule VisitExpr_BracketExpression([NotNull] Predicate1Parser.Expr_BracketExpressionContext context)
        {
            return Visit(context.expr());
        }

        public override ParsedRule VisitVariable([NotNull] Predicate1Parser.VariableContext context)
        {
            string variableName = context.GetText();

            var variableType = metadata.GetRuleDataType(variableName);

            if (!variableType.HasValue)
            {
                throw new RuleEvaluatorException("No metadata defined for field : "+ variableName);
            }

            return ParsedRule.Parameter(variableName, variableType.Value);
        }

        public override ParsedRule VisitNumber([NotNull] Predicate1Parser.NumberContext context)
        {
            return ParsedRule.Constant(decimal.Parse(context.GetText()));
        }
    }
}
