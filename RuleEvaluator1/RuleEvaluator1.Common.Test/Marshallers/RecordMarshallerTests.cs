using FluentAssertions;
using RuleEvaluator1.Common.Marshallers;
using System.IO;
using System.Text.Json;
using Xunit;

namespace RuleEvaluator1.Common.Test.Marshallers
{
    public class RecordMarshallerTests
    {
        protected readonly RecordMarshaller subject;
        protected readonly JsonSerializerOptions serializerOptions;

        public RecordMarshallerTests()
        {
            this.subject = new RecordMarshaller();
            serializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    subject
                }
            };
        }

        [Fact]
        public void Should_deserialize_simple_record_json()
        {
            // arrange
            string input = File.ReadAllText("Marshallers/Data/SimpleRecord.json");

            // act
            var result = JsonSerializer.Deserialize<Common.Models.Record>(input, serializerOptions);

            // assure
            result.Get<decimal>("key1").Should().Be(10m);
            result.Get<bool>("key2").Should().Be(true);
        }

        [Fact]
        public void Should_deserialize_nested_record_json()
        {
            // arrange
            string input = File.ReadAllText("Marshallers/Data/NestedRecord.json");

            // act
            var result = JsonSerializer.Deserialize<Common.Models.Record>(input, serializerOptions);

            // assure
            result.Get<decimal>("key5.key52").Should().Be(10.20m);
        }
    }
}
