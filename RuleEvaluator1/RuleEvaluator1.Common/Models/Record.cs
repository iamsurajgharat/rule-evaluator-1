using RuleEvaluator1.Common.Marshallers;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RuleEvaluator1.Common.Models
{
    [JsonConverter(typeof(RecordMarshaller))]
    public class Record
    {
        private readonly Dictionary<string, object> data;

        public Record()
        {
            this.data = new Dictionary<string, object>();
        }

        public Record(Dictionary<string, object> data) : this()
        {
            if (data == null)
            {
                return;
            }

            foreach (var item in data)
            {
                Set(item.Key, item.Value);
            }
        }

        public Dictionary<string, object> GetDataAsDictionary()
        {
            return this.data;
        }

        public T Get<T>(string fieldName)
        {
            return (T)Get(fieldName, typeof(T));
        }

        public object Get(string fieldName, Type typeOfData)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return null;
            }

            int dotIndex = fieldName.IndexOf(".");

            if (dotIndex > 0)
            {
                string parentFieldName = fieldName.Substring(0, dotIndex);
                string remainingFieldName = fieldName.Substring(dotIndex + 1);

                if (data.ContainsKey(parentFieldName) && (data[parentFieldName] is Record parentData))
                {
                    return parentData.Get(remainingFieldName, typeOfData);
                }

                return GetDefaultValue(typeOfData);
            }
            else if (data.ContainsKey(fieldName))
            {
                return Convert.ChangeType(data[fieldName], typeOfData);
            }

            return GetDefaultValue(typeOfData);
        }

        public void Set(string fieldName, object value)
        {
            int dotIndex = fieldName.IndexOf(".");

            if (dotIndex > 0)
            {
                string parentFieldName = fieldName.Substring(0, dotIndex);
                string remainingFieldName = fieldName.Substring(dotIndex + 1);

                if (!string.IsNullOrWhiteSpace(parentFieldName) && !string.IsNullOrWhiteSpace(remainingFieldName))
                {
                    Record nestedRecord = Get<Record>(parentFieldName);

                    if (nestedRecord == null)
                    {
                        nestedRecord = new Record();
                        this.data[parentFieldName] = nestedRecord;
                    }

                    nestedRecord.Set(remainingFieldName, value);
                }
            }
            else
            {
                if (typeof(Dictionary<string, object>) == value.GetType())
                {
                    this.data[fieldName] = (Record)(Dictionary<string, object>)value;
                }
                else
                {
                    this.data[fieldName] = value;
                }

            }
        }

        private object GetDefaultValue(Type t)
        {
            return (t.IsValueType) ? Activator.CreateInstance(t) : null;
        }

        public static explicit operator Record(Dictionary<string, object> data)
        {
            return new Record(data);
        }
    }
}
