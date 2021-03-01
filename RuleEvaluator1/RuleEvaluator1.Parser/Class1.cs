using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;

namespace RuleEvaluator1.Parser
{
    public class Class1
    {
        public static void Test1()
        {
            String input = "hello suraj";
           
            //MyCustomListener mcl = new MyCustomListener();
            //ParseTreeWalker.Default.Walk(mcl, tree);
        }

        public static void Test2()
        {
            String input = "10 > 20";
            ICharStream stream = CharStreams.fromstring(input);
            ITokenSource lexer = new Predicate1Lexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            Predicate1Parser parser = new Predicate1Parser(tokens);
            parser.BuildParseTree = true;
            var ttt = parser.pexpr();
        }
    }
}
