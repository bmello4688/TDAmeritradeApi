using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeApi.Client
{
    internal class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));

            if (reader.TokenType == JsonTokenType.String)
            {
                if (!reader.TryGetDateTime(out DateTime value))
                {
                    value = DateTime.Parse(reader.GetString());
                }

                return value;
            }
            else if (reader.TokenType == JsonTokenType.Number)//long since epoch time
            {
                var offset = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());

                return offset.DateTime;
            }
            else
                throw new NotSupportedException(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("u"));
        }
    }

    internal class JsonDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTimeOffset));

            if (reader.TokenType == JsonTokenType.String)
            {
                if (!reader.TryGetDateTimeOffset(out DateTimeOffset value))
                {
                    value = DateTimeOffset.Parse(reader.GetString());
                }

                return value;
            }
            else if (reader.TokenType == JsonTokenType.Number)//long since epoch time
            {
                var offset = DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());

                return offset;
            }
            else
                throw new NotSupportedException(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("u"));
        }
    }
}