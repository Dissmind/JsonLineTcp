using System.Text.Json.Serialization;

namespace JsonLineTcp.Server;

public sealed class ServerConfig
{
    [JsonPropertyName("ListenAddr")] 
    public string ListenAddr { get; init; } = "0.0.0.0";
    [JsonPropertyName("ListenPort")] 
    public int ListenPort { get; init; } = 5555;

    [JsonPropertyName("TlsEnabled")] 
    public bool TlsEnabled { get; init; }
    [JsonPropertyName("TlsCrt")] 
    public string? TlsCrt { get; init; }
    [JsonPropertyName("TlsKey")] 
    public string? TlsKey { get; init; }

    [JsonPropertyName("Filters")] 
    public List<FilterConfig> Filters { get; init; } = new();
}