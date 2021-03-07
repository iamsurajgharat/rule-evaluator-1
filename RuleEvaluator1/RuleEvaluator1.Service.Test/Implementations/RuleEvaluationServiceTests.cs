using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit;
using FluentAssertions;
using Moq;
using RuleEvaluator1.Common.Enums;
using RuleEvaluator1.Common.Exceptions;
using RuleEvaluator1.Common.Helpers;
using RuleEvaluator1.Common.Models;
using RuleEvaluator1.Service.Implementations;
using RuleEvaluator1.Service.Interfaces;
using RuleEvaluator1.Service.Messages;
using RuleEvaluator1.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RuleEvaluator1.Service.Test.Implementations
{
    public class RuleEvaluationServiceTests : TestKit
    {
        protected readonly RuleEvaluationService objectUnderTest;

        protected readonly Mock<IActorProviderService> actorProviderServiceMock;

        public RuleEvaluationServiceTests()
        {
            actorProviderServiceMock = new Mock<IActorProviderService>();
            this.objectUnderTest = new RuleEvaluationService(actorProviderServiceMock.Object);
        }

        public class PutMetadataAsync : RuleEvaluationServiceTests
        {
            [Fact]
            public async Task Should_handle_null_input_aptly()
            {
                // act
                await objectUnderTest.PutMetadataAsync(null);

                // assure
                actorProviderServiceMock.Verify(x => x.GetRuleManagerActor(), Times.Never);
            }

            [Fact]
            public async Task Should_send_save_request_to_rule_manager_actor_for_valid_input()
            {
                // create probe to act as rule manager actor and respond like him
                var probe1 = CreateTestProbe("MyProbe1");
                probe1.SetAutoPilot(new DelegateAutoPilot((sender, message) =>
                {
                    // ensure receive message is correct
                    message.Should().BeOfType(typeof(SaveMetadataRequest));

                    // respond back
                    sender.Tell(new BaseAckResponse(), ActorRefs.NoSender);

                    // No need to handle more messages
                    return AutoPilot.NoAutoPilot;
                }));
                actorProviderServiceMock.Setup(x => x.GetRuleManagerActor()).Returns(probe1.Ref);

                // arrange
                var metadata = new Dictionary<string, string>
                {
                    ["Field1"] = "Number",
                    ["Field2"] = "Text"
                };

                // act
                await objectUnderTest.PutMetadataAsync(metadata);
            }

            [Fact]
            public async Task Should_throw_exception_for_non_success_rule_manager_actor_response()
            {
                // create probe to act as rule manager actor and respond like him
                var probe1 = CreateTestProbe("MyProbe1");
                probe1.SetAutoPilot(new DelegateAutoPilot((sender, message) =>
                {
                    // ensure receive message is correct
                    message.Should().BeOfType(typeof(SaveMetadataRequest));

                    // respond back
                    sender.Tell(new BaseAckResponse { Message = "Buzz off!", IsSuccess = false }, ActorRefs.NoSender);

                    // No need to handle more messages
                    return AutoPilot.NoAutoPilot;
                }));
                actorProviderServiceMock.Setup(x => x.GetRuleManagerActor()).Returns(probe1.Ref);

                // arrange
                var metadata = new Dictionary<string, string>();

                // act and assure
                objectUnderTest.Invoking(async y => await y.PutMetadataAsync(metadata)).Should().Throw<RuleEvaluatorException>().WithMessage("Buzz off!");
            }

            [Fact]
            public async Task Should_throw_exception_for_invalid_data_types()
            {
                // arrange
                var metadata = new Dictionary<string, string>
                {
                    ["Field1"] = "Number",
                    ["Field2"] = "Text",
                    ["Field3"] = "InvalidDataType"
                };

                // expected error message
                string errorMessage = string.Format(Common.Constants.Messages.InvalidRuleDataTypes, string.Join(',', Utility.List("InvalidDataType")),
                    string.Join(',', Enum.GetValues(typeof(RuleDataType))));

                // act and assure
                objectUnderTest.Invoking(async y => await y.PutMetadataAsync(metadata)).Should().Throw<RuleEvaluatorException>().WithMessage(errorMessage);
                actorProviderServiceMock.Verify(x => x.GetRuleManagerActor(), Times.Never);
            }
        }
    }
}
