using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Web.Controllers;
using RuleEvaluator1.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RuleEvaluator1.Web.Test.Controllers
{
    public class RuleControllerTests
    {
        protected RuleController subject;
        protected Mock<IRuleEvaluationService> ruleEvaluationServiceMock;
        protected IMapper mapper;

        public RuleControllerTests()
        {
            mapper = AutoMapperConfiguration.GetMapper();
            ruleEvaluationServiceMock = new Mock<IRuleEvaluationService>();

            subject = new RuleController(mapper, ruleEvaluationServiceMock.Object);
        }

        public class PutMetadataAsync : RuleControllerTests
        {
            [Fact]
            public async Task Should_return_ok_with_success_for_valid_input()
            {
                // act
                var response = await subject.PutMetadataAsync(new Dictionary<string, string> { { "field1", "Number" } });

                // assure
                response.Should().NotBeNull();
                response.Result.Should().BeOfType<OkObjectResult>();
                var okResult = response.Result as OkObjectResult;
                okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
                okResult.Value.Should().BeOfType<WebApiResponse>();
                (okResult.Value as WebApiResponse).Should().Be(WebApiResponse.SuccessInstance);

            }

            [Fact]
            public async Task Should_return_bad_request_for_empty_input()
            {
                // act
                var response = await subject.PutMetadataAsync(new Dictionary<string, string>());

                // assure
                response.Should().NotBeNull();
                response.Result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = response.Result as BadRequestObjectResult;
                badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                badRequestResult.Value.Should().BeOfType<WebApiResponse>();
                (badRequestResult.Value as WebApiResponse).Should().Be(WebApiResponse.EmptyInputInstance);

            }
        }

        public class PutAsync : RuleControllerTests
        {
            [Fact]
            public async Task Should_return_ok_with_success_for_valid_input()
            {
                // arrange
                Rule rule = new Rule
                {
                    PredicateExpression = "A == 100",
                    Result = "id123"
                };

                // setup mock response and assure passed input
                ruleEvaluationServiceMock.Setup(x => x.AddUpdateRulesAsync(It.IsAny<IEnumerable<InputRule>>())).ReturnsAsync((IEnumerable<InputRule> x) =>
                {
                    // assure passed input
                    x.Should().HaveCount(1);
                    x.First().Id.Should().Be("rule1");

                    return new Dictionary<string, BaseAckResponse>
                    {
                        {"rule1", new BaseAckResponse{ IsSuccess = true, Message = Common.Constants.Messages.SucessAck } }
                    };
                }).Verifiable();

                // act
                var response = await subject.PutAsync("rule1", rule);

                // assure
                response.Should().NotBeNull();
                response.Result.Should().BeOfType<OkObjectResult>();
                var okResult = response.Result as OkObjectResult;
                okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
                okResult.Value.Should().BeOfType<WebApiResponse>();
                ruleEvaluationServiceMock.Verify();
            }

            [Fact]
            public async Task Should_return_bad_request_for_invalid_input()
            {
                // arrange
                Rule rule = new Rule
                {
                    PredicateExpression = "A == 100",
                    Result = "id123"
                };

                // setup mock response and assure passed input
                ruleEvaluationServiceMock.Setup(x => x.AddUpdateRulesAsync(It.IsAny<IEnumerable<InputRule>>())).ReturnsAsync((IEnumerable<InputRule> x) =>
                {
                    // assure passed input
                    x.Should().HaveCount(1);
                    x.First().Id.Should().Be("rule1");

                    return new Dictionary<string, BaseAckResponse>
                    {
                        {"rule1", new BaseAckResponse{ IsSuccess = false, Message = Common.Constants.Messages.InvalidRuleDataTypes } }
                    };
                }).Verifiable();

                // act
                var response = await subject.PutAsync("rule1", rule);

                // assure
                response.Should().NotBeNull();
                response.Result.Should().BeOfType<BadRequestObjectResult>();
                var badRequestResult = response.Result as BadRequestObjectResult;
                badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                badRequestResult.Value.Should().BeOfType<WebApiResponse>();
                ruleEvaluationServiceMock.Verify();
            }
        }
    }
}
