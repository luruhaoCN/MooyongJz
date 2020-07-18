using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MooyongCommon
{
    public class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.GetString().Trim() != "")
                {
                    if (DateTime.TryParse(reader.GetString(), out DateTime date))
                        return date;
                }
                else
                {
                    return new DateTime();
                }
            }
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
