using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Web.Models;

namespace RuleEvaluator1.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly bool isDevelopment;

        public ErrorController(IHostEnvironment hostingEnvironment)
        {
            isDevelopment = hostingEnvironment.IsDevelopment();
        }

        /// <summary>
        /// Error Action Method to return JSON formatted error detail.
        /// </summary>
        /// <returns></returns>
        [Route("/error")]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            var response = new WebApiResponse();
            int statusCode = StatusCodes.Status200OK;

            if (ex != null)
            {
                if (ex is RuleEvaluatorException)
                {
                    statusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    statusCode = StatusCodes.Status500InternalServerError;
                }

                response.AckMessage = ex.Message;

                if (isDevelopment || HttpContext.Request.Headers.Keys.Contains("Trace"))
                {
                    response.AdditionalDetails = ex.ToString();
                }
            }

            return StatusCode(statusCode, response);
        }
    }
}