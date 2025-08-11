using System.Text.Json;

namespace JsonLineTcp.Server;

public interface IMessageHandler
{
    Task HandleAsync(JsonElement json, CancellationToken ct);
}