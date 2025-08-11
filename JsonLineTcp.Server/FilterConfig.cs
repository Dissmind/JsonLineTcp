using System.Text.Json.Serialization;


namespace JsonLineTcp.Server;

public sealed class FilterConfig
{
    [JsonPropertyName("field")] 
    public string Field { get; init; } = string.Empty;
    
    /// <summary>
    /// equals, not_equals, contains, not_contains
    /// </summary>
    [JsonPropertyName("operator")] 
    public string Operator { get; init; } 
   
    [JsonPropertyName("value")] 
    public string? Value { get; init; }
}