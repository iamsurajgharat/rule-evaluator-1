namespace RuleEvaluator1.Service.Models
{
    public class RuleShard
    {
        public int Number { get; private set; }
        static RuleShard()
        {
            
        }
        
        private RuleShard() { }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RuleShard))
            {
                return false;
            }

            return Number.Equals((obj as RuleShard).Number);
        }

        public override string ToString()
        {
            return Number.ToString();
        }

        public static explicit operator RuleShard(int number)
        {
            return new RuleShard
            {
                Number = number
            };
        }
    }
}
