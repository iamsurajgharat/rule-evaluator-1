using FluentAssertions;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Web.Models;
using System.Collections.Generic;
using Xunit;

namespace RuleEvaluator1.Web.Test.Models
{
    public class WebApiResponseTests
    {
        public class ExplicitConversion_Dic_String_BaseAckResponse : WebApiResponseTests
        {
            [Fact]
            public void Should_return_success_for_null_input()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = null;

                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.IsBadRequest().Should().BeFalse();
            }

            [Fact]
            public void Should_return_success_for_empty_input()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = new Dictionary<string, BaseAckResponse>();

                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.IsBadRequest().Should().BeFalse();
            }

            [Fact]
            public void Should_return_success_for_all_success_entries()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = new Dictionary<string, BaseAckResponse>();
                input.Add("r1", new BaseAckResponse());
                input.Add("r2", new BaseAckResponse());
                input.Add("r3", new BaseAckResponse());

                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.IsBadRequest().Should().BeFalse();
            }

            [Fact]
            public void Should_return_as_bad_request_for_all_failed_entries()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = new Dictionary<string, BaseAckResponse>();
                input.Add("r1", new BaseAckResponse { IsSuccess = false });
                input.Add("r2", new BaseAckResponse { IsSuccess = false });
                input.Add("r3", new BaseAckResponse { IsSuccess = false });
                

                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.IsBadRequest().Should().BeTrue();
            }

            [Fact]
            public void Should_return_neither_success_nor_bad_request_for_mix_input()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = new Dictionary<string, BaseAckResponse>();
                input.Add("r1", new BaseAckResponse { IsSuccess = true });
                input.Add("r2", new BaseAckResponse { IsSuccess = false });
                input.Add("r3", new BaseAckResponse { IsSuccess = true });
                input.Add("r4", new BaseAckResponse { IsSuccess = false });


                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
                result.IsBadRequest().Should().BeFalse();
            }

            [Fact]
            public void Should_return_result_as_passed_input()
            {
                // arrange
                Dictionary<string, BaseAckResponse> input = new Dictionary<string, BaseAckResponse>();
                input.Add("r1", new BaseAckResponse { IsSuccess = true });
                input.Add("r2", new BaseAckResponse { IsSuccess = false });
                input.Add("r3", new BaseAckResponse { IsSuccess = true });
                input.Add("r4", new BaseAckResponse { IsSuccess = false });


                // act
                WebApiResponse result = (WebApiResponse)input;

                // assure
                result.Should().NotBeNull();
                (result.Result is Dictionary<string, BaseAckResponse>).Should().BeTrue();
                (result.Result as Dictionary<string, BaseAckResponse>).Should().HaveCount(4);
            }
        }

        public class EmptyInputInstance : WebApiResponseTests
        {
            [Fact]
            public void Should_return_response_instance_for_empty_input_request()
            {
                // act
                var result = WebApiResponse.EmptyInputInstance;

                // assure
                result.IsSuccess.Should().BeFalse();
                result.AckMessage.Should().Be(Common.Constants.Messages.EmptyRequest);
                result.IsBadRequest().Should().BeTrue();
            }
        }

        public class SuccessInstance : WebApiResponseTests
        {
            [Fact]
            public void Should_return__default_response_for_success()
            {
                // act
                var result = WebApiResponse.SuccessInstance;

                // assure
                result.IsSuccess.Should().BeTrue();
                result.AckMessage.Should().Be(Common.Constants.Messages.SucessAck);
                result.IsBadRequest().Should().BeFalse();
            }
        }
    }
}
