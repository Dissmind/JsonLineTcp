using System.Text.Json;
using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Server;

public class FilterProcessor(IJsonValidator validator, IEnumerable<IFilter> filters)
{
    private readonly IJsonValidator _validator = validator;
    private readonly IFilter[] _filters = filters.ToArray();

    public bool Passes(ReadOnlyMemory<byte> json)
    {
        if (!_validator.IsValid(json))
        {
            return false;
        }
        
        using JsonDocument doc = JsonDocument.Parse(json);
        
        JsonElement root = doc.RootElement;
        
        foreach (IFilter filter in _filters)
        {
            if (!filter.Match(root))
            {
                return false;
            }
        }
        
        return true;
    }
}