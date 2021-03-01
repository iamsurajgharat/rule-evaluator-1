using System;

namespace RuleEvaluator1.Common.Exceptions
{
    public class RuleEvaluatorException : Exception
    {
        public RuleEvaluatorException(string message) : base(message)
        {

        }

        public RuleEvaluatorException(Exception ex) : base(ex.Message, ex)
        {

        }

        public RuleEvaluatorException()
        {
        }

        public RuleEvaluatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
