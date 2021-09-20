using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Msoop.Web.Reddit.Serialization
{
    public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)reader.GetDouble());
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
