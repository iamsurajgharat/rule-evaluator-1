using RuleEvaluator1.Common.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuleEvaluator1.Common.Marshallers
{
    public class RecordMarshaller : JsonConverter<Record>
    {
        public override Record Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var data = ReadObject(ref reader);

            return new Record(data);
        }

        public override void Write(Utf8JsonWriter writer, Record value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(JsonSerializer.Serialize(value.GetDataAsDictionary()));
        }

        private Dictionary<string, object> ReadObject(ref Utf8JsonReader reader)
        {
            var data = new Dictionary<string, object>();
            string propName = string.Empty;
            while (reader.Read())
            {
                if (JsonTokenType.EndObject == reader.TokenType)
                {
                    break;
                }

                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        propName = reader.GetString();
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        data[propName] = reader.GetBoolean();
                        break;
                    case JsonTokenType.Number:
                        data[propName] = reader.GetDecimal();
                        break;
                    case JsonTokenType.String:
                        data[propName] = reader.GetString();
                        break;
                    case JsonTokenType.StartObject:
                        data[propName] = ReadObject(ref reader);
                        break;
                }
            }

            return data;
        }
    }
}
