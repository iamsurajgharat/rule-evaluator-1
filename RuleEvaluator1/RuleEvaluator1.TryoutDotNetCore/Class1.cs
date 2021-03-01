using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuleEvaluator1.TryoutDotNetCore
{
    public class Class1 : JsonConverter<object>
    {
        private DateTimeOffset date;
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
