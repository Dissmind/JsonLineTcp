using System.Text.Json;

namespace JsonLineTcp.Server;

public interface IFilter
{
    bool Match(JsonElement json);
}