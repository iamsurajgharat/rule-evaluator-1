using Antlr4.Runtime;
using FluentAssertions;
using Newtonsoft.Json;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Parser;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace RuleEvaluator1.Service.Test
{
    public class UnitTest1
    {
        [Fact(Skip ="Not valid anymore")]
        public void Test1()
        {
            int t = 10+-20 * 30;
            var c1 = Expression.Constant(10);
            var s1 = JsonConvert.SerializeObject(c1);
            var c2 = JsonConvert.DeserializeObject<ConstantExpression>(s1);
        }

        [Fact(Skip = "Not valid anymore")]
        public void Test2()
        {
            String input = "xyz > 20";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new Predicate1Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Predicate1Parser parser = new Predicate1Parser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.pexpr();
            var visitor = new MyCustomVisitor2();
            var result = visitor.Visit(tree);
            var paramsl = new List<ParameterExpression>();
            Expression cex = result.Compile(new Dictionary<string, RuleDataType>(), paramsl, typeof(bool));

            Delegate cex2 = Expression.Lambda(cex, paramsl).Compile();

            var r1 = (bool)cex2.DynamicInvoke(Convert.ChangeType(40, paramsl[0].Type));
            r1.Should().BeTrue();

            var r2 = (bool)cex2.DynamicInvoke(Convert.ChangeType(10, paramsl[0].Type));
            r2.Should().BeFalse();
        }

        [Fact(Skip = "Not valid anymore")]
        public void Test3()
        {
            String input = "(xyz + abc) > 20";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new Predicate1Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Predicate1Parser parser = new Predicate1Parser(tokens);
            parser.BuildParseTree = true;
            var tree = parser.pexpr();
            var visitor = new MyCustomVisitor2();
            var result = visitor.Visit(tree);
            var paramsl = new List<ParameterExpression>();
            Expression cex = result.Compile(new Dictionary<string, RuleDataType>(), paramsl, typeof(bool));

            Delegate cex2 = Expression.Lambda(cex, paramsl).Compile();

            var r1 = (bool)cex2.DynamicInvoke(10m, 20m);
            r1.Should().BeTrue();

            var r2 = (bool)cex2.DynamicInvoke(5m, 15m);
            r2.Should().BeFalse();

            var r3 = (bool)cex2.DynamicInvoke(new object[] { 25m, 15m});
            r3.Should().BeTrue();
        }
    }
}
