using AutoMapper;
using RuleEvaluator1.Common.Models;

namespace RuleEvaluator1.Web
{
    public class AutoMapperConfiguration
    {
        private static IMapper mapper;
        public static MapperConfiguration Configure()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Models.Rule, InputRule>();


                cfg.CreateMap<InputRule, Models.Rule>();
            });
        }

        public static IMapper GetMapper()
        {
            if (mapper == null)
            {
                mapper = Configure().CreateMapper();
            }

            return mapper;
        }
    }
}
