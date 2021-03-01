using Akka.Actor;
using RuleEvaluator1.Service.Interfaces;
using System;

namespace RuleEvaluator1.Service.Implementations
{
    public class ActorProviderService : IActorProviderService
    {
        private readonly IActorRef ruleActorShardRegionProxy;
        private readonly IActorRef ruleManagerActor;
        public ActorProviderService(IActorRef ruleManagerActor, IActorRef ruleActorShardRegionProxy)
        {
            this.ruleManagerActor = ruleManagerActor ?? throw new ArgumentNullException(nameof(ruleManagerActor));
            this.ruleActorShardRegionProxy = ruleActorShardRegionProxy ?? throw new ArgumentNullException(nameof(ruleActorShardRegionProxy));
        }

        public IActorRef GetRuleActorShardRegionProxy()
        {
            return ruleActorShardRegionProxy;
        }

        public IActorRef GetRuleManagerActor()
        {
            return ruleManagerActor;
        }
    }
}
