using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Msoop.Reddit
{
    public class DiscussionUrlConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var discussionPath = reader.GetString();
            return $"https://www.reddit.com{discussionPath}";
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
