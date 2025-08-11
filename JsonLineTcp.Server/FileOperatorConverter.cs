using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLineTcp.Server;

public class FileOperatorConverter
{
    public sealed class FilterOperatorConverter : JsonConverter<FilterOperator>
    {
        public override FilterOperator Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString() ?? throw new JsonException("operator is null");
            string norm = new string(s.Where(char.IsLetter).ToArray()).ToLowerInvariant();

            return norm switch
            {
                "equals"      => FilterOperator.Equals,
                "notequals"   => FilterOperator.NotEquals,
                "contains"    => FilterOperator.Contains,
                "notcontains" => FilterOperator.NotContains,
                _ => throw new JsonException($"Unsupported operator: {s}")
            };
        }

        public override void Write(Utf8JsonWriter writer, FilterOperator value, JsonSerializerOptions options)
        {
            string str = value switch
            {
                FilterOperator.Equals      => "equals",
                FilterOperator.NotEquals   => "not_equals",
                FilterOperator.Contains    => "contains",
                FilterOperator.NotContains => "not_contains",
                _ => throw new JsonException($"Unsupported operator: {value}")
            };
            writer.WriteStringValue(str);
        }
    }
}