using System.Collections.Generic;
using System.Linq;

namespace RuleEvaluator1.Common.Helpers
{
    public static class Utility
    {
        public static List<T> List<T>(params T[] args) => args == null ? new List<T>() : args.ToList();
    }
}
