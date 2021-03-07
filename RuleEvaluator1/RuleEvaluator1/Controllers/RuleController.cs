using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RuleEvaluator1.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IRuleEvaluationService ruleEvaluationService;
        private readonly IMapper mapper;

        public RuleController(IMapper mapper, IRuleEvaluationService ruleEvaluationService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.ruleEvaluationService = ruleEvaluationService ?? throw new ArgumentNullException(nameof(ruleEvaluationService));
        }

        // GET: api/Rule
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Map<InputRule, Rule>(ruleEvaluationService.GetAllRules()));
        }

        // GET: api/Rule/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(string id)
        {
            var rule = ruleEvaluationService.GetRules(List(id)).SingleOrDefault();
            if (rule == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(Map<InputRule, Rule>(rule));
            }
        }

        [HttpPost("/eval")]
        [Consumes("application/json")]
        public async Task<IActionResult> EvalAsync([FromBody] List<Record> records)
        {
            var result = await ruleEvaluationService.EvaluateAsync(records);

            return Ok(result);
        }

        // PUT: api/Rule/5
        [HttpPut("{id}")]
        public async Task<object> PutAsync(string id, [FromBody] Rule value)
        {
            Dictionary<string, Service.Messages.BaseAckResponse> result = await ruleEvaluationService.AddUpdateRulesAsync(List(Map<Rule, InputRule>(value)));
            var response = new WebApiResponse
            {
                Result = result,
                IsSuccess = result.Values.All(x => x.IsSuccess)
            };

            return Ok(result);
        }

        
        [HttpPut("/metadata")]
        public async Task<object> PutMetadata([FromBody] Dictionary<string,string> metadata)
        {
            if (metadata == null)
            {
                return BadRequest(WebApiResponse.EmptyInputInstance);
            }

            await ruleEvaluationService.PutMetadataAsync(metadata);

            return Ok(WebApiResponse.SuccessInstance);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
        }

        private List<T> List<T>(params T[] args)
        {
            return args == null ? new List<T>() : args.ToList();
        }

        private IEnumerable<T2> Map<T1,T2>(IEnumerable<T1> values)
        {
            foreach (var item in values)
            {
                yield return Map<T1,T2>(item);
            }
        }

        private T2 Map<T1, T2>(T1 value)
        {
            return mapper.Map<T2>(value);
        }
    }
}
