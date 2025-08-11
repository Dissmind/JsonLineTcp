using System.Text.Json.Serialization;

namespace JsonLineTcp.Client.Intrefaces;

public class ClientConfig
{
    [JsonPropertyName("InputPath")] 
    public string InputPath { get; init; } = string.Empty;
    
    [JsonPropertyName("ServerHost")] 
    public string ServerHost { get; init; } = "localhost";
    
    [JsonPropertyName("Server_port")] 
    public int ServerPort { get; init; } = 5555;
    
    [JsonPropertyName("TlsEnabled")] 
    public bool TlsEnabled { get; init; }
    
    [JsonPropertyName("TlsValidateCert")] 
    public bool TlsValidateCert { get; init; }
}