using Akka.Actor;

namespace RuleEvaluator1.Service.Interfaces
{
    public interface IActorProviderService
    {
        IActorRef GetRuleManagerActor();

        IActorRef GetRuleActorShardRegionProxy();
    }
}
