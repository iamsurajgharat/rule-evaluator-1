using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace RuleEvaluator1.Common.Test.Models
{
    public class RecordTests
    {
        public class Get : RecordTests
        {
            [Fact]
            public void Should_return_primitive_values_correctly()
            {
                // arrange
                var data = new Dictionary<string, object>();
                data["key1"] = 10;
                data["key2"] = 10.20;
                data["key3"] = 12.35m;
                data["key4"] = true;
                data["key5"] = false;
                data["key6"] = "Text Message";

                var record = new Common.Models.Record(data);

                // act and assure
                record.Get<decimal>("key1").Should().Be(10);
                record.Get<decimal>("key2").Should().Be(10.20m);
                record.Get<decimal>("key3").Should().Be(12.35m);
                record.Get<bool>("key4").Should().Be(true);
                record.Get<bool>("key5").Should().Be(false);
                record.Get<string>("key6").Should().Be("Text Message");

            }

            [Fact]
            public void Should_return_nested_values_correctly()
            {
                // arrange
                var data = new Dictionary<string, object>();
                data["key7.key71"] = "Nested value!";

                var record = new Common.Models.Record(data);

                // act and assure
                record.Get<string>("key7.key71").Should().Be("Nested value!");

            }
        }

        public class Set : RecordTests
        {
            [Fact]
            public void Should_set_primitive_values_correctly()
            {
                // arrange
                var record = new Common.Models.Record();

                // act
                record.Set("key1", 10);
                record.Set("key2", 10.20m);
                record.Set("key3", true);
                record.Set("key4", "TextMessage");

                // act and assure
                record.Get<decimal>("key1").Should().Be(10);
                record.Get<decimal>("key2").Should().Be(10.20m);
                record.Get<bool>("key3").Should().Be(true);
                record.Get<string>("key4").Should().Be("TextMessage");
            }

            [Fact]
            public void Should_set_nested_values_correctly()
            {
                // arrange
                var record = new Common.Models.Record();

                // act
                record.Set("key1.key11", 10.20);

                // act and assure
                record.Get<decimal>("key1.key11").Should().Be(10.20m);
            }
        }
    }
}
