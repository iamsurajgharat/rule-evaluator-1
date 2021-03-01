using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Configuration;
using Akka.DI.Core;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Actors;
using RuleEvaluator1.Service.Helpers;
using RuleEvaluator1.Service.Implementations;
using RuleEvaluator1.Service.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RuleEvaluator1
{
    public class Startup
    {
        private ActorSystem actorSystem;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddMvc().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.Converters.Add(new ObjectToInferredTypesMarshaller());
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            actorSystem.UseAutofac(app.ApplicationServices.GetAutofacRoot());
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var config = ParseConfig("akka.hocon");

            actorSystem = ActorSystem.Create("REActorSystem", config.WithFallback(ClusterSharding.DefaultConfig())
                                                                                                .WithFallback(DistributedPubSub.DefaultConfig()));

            // Create and build container
            var lazyActorSys = new Lazy<ActorSystem>(() => { return actorSystem; });
            builder.Register(x => lazyActorSys.Value).SingleInstance();

            builder.Register<IActorProviderService>(x =>
            {
                var system = x.Resolve<ActorSystem>();
                var ruleManagerActorProps = system.DI().Props<RuleManagerActor>();
                var ruleManagerActor = system.ActorOf(ruleManagerActorProps);
                var ruleActorShardRegionProxy = GetShardRegionProxyForRuleActor(actorSystem).GetAwaiter().GetResult();
                return new ActorProviderService(ruleManagerActor, ruleActorShardRegionProxy);
            }).SingleInstance();

            builder.RegisterType<RuleManagerActor>();
            builder.RegisterType<RuleActor>();
            builder.RegisterType<RuleEvaluationService>().As<IRuleEvaluationService>();
            builder.Register(x => GetMapper());
        }

        private async Task<IActorRef> GetShardRegionProxyForRuleActor(ActorSystem actorSystem)
        {
            return await ClusterSharding.Get(actorSystem).StartAsync(
                    typeName: "RuleEvaluator1.Service.Actors.RuleActor",
                    entityProps: Props.Create<RuleActor>(),
                    settings: ClusterShardingSettings.Create(actorSystem),
                    messageExtractor: new RuleActorShardingMessageExtractor(10));
        }

        private Config ParseConfig(string hoconPath)
        {
            return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
        }


        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Web.Models.Rule, InputRule>();


                cfg.CreateMap<InputRule, Web.Models.Rule>();
            });

            return config.CreateMapper();
        }
    }
}
