using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TDAmeritradeApi.Client
{
    public class JsonAutoStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long number))
                {
                    return number.ToString(CultureInfo.InvariantCulture);
                }
                else if (reader.TryGetDouble(out var doubleNumber))
                {
                    return doubleNumber.ToString(CultureInfo.InvariantCulture);
                }
                else
                    throw new NotSupportedException(reader.GetString());

            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.False || reader.TokenType == JsonTokenType.True)
            {
                return reader.GetBoolean().ToString();
            }
            else
            {
                var doc = JsonDocument.ParseValue(ref reader);

                return doc.RootElement.ToString();
            }
        }

        private static string GetText(Utf8JsonReader reader)
        {
            byte[] sequence = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();

            return Encoding.UTF8.GetString(sequence);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
