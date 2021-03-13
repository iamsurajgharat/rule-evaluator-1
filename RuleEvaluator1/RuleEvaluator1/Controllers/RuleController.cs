using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RuleEvaluator1.Common.Helpers;
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

        [HttpGet("shard")]
        public async Task<ActionResult<WebApiResponse>> GetRuleShards()
        {
            return Ok(WebApiResponse.Instance(await ruleEvaluationService.GetRuleShardsAsync()));
        }


        // GET: api/Rule
        /*[HttpGet]
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
        }*/

        [HttpPost("eval")]
        [Consumes("application/json")]
        public async Task<IActionResult> EvalAsync([FromBody] List<Record> records)
        {
            var result = await ruleEvaluationService.EvaluateAsync(records);

            return Ok(result);
        }

        // PUT: api/Rule/5
        [HttpPut("{id}")]
        public async Task<ActionResult<WebApiResponse>> PutAsync(string id, [FromBody] Rule value)
        {
            // set id
            value.Id = id;

            return await AddUpdateRulesAsync(Utility.List(value));

        }

        [HttpPost]
        public async Task<ActionResult<WebApiResponse>> AddUpdateRulesAsync([FromBody] List<Rule> rules)
        {
            if (rules == null || rules.Count == 0)
            {
                return BadRequest(WebApiResponse.EmptyInputInstance);
            }

            // perform add/update
            WebApiResponse response = (WebApiResponse)await ruleEvaluationService.AddUpdateRulesAsync(Map<Rule, InputRule>(rules));

            return response.IsBadRequest() ? BadRequest(response) : (ActionResult<WebApiResponse>)Ok(response);
        }

        [HttpPut("metadata")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<WebApiResponse>> PutMetadataAsync([FromBody] Dictionary<string, string> metadata)
        {
            if (metadata.Count == 0)
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

        private IEnumerable<T2> Map<T1, T2>(IEnumerable<T1> values)
        {
            foreach (var item in values)
            {
                yield return Map<T1, T2>(item);
            }
        }

        private T2 Map<T1, T2>(T1 value)
        {
            return mapper.Map<T2>(value);
        }
    }
}
