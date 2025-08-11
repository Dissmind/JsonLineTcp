using System.Text.Json;


namespace JsonLineTcp.Server;



public class FilterFactory
{
    public static IFilter Create(FilterConfig config)
    {
        return config.Operator switch
        {
            "equals"      => new EqualsFilter(config.Field, config.Value),
            "not_equals"   => new NotEqualsFilter(config.Field, config.Value),
            "contains"    => new ContainsFilter(config.Field, config.Value ?? string.Empty),
            "not_contains" => new NotContainsFilter(config.Field, config.Value ?? string.Empty),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static string NormalizeOp(string op)
        => op.Replace("-", "_", StringComparison.Ordinal)
             .Replace(" ", "_", StringComparison.Ordinal)
             .ToLowerInvariant();

    private abstract class BaseFilter(string field)
        : IFilter
    {
        protected readonly string Field = field;
        protected static bool TryGetTopLevelProperty(JsonElement root, string name, out JsonElement value)
        {
            if (root.ValueKind != JsonValueKind.Object)
            {
                value = default; 
                
                return false;
            }
            
            foreach (var p in root.EnumerateObject())
            {
                if (string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = p.Value;
                    return true;
                }
            }
            
            value = default;
            
            return false;
        }

        public abstract bool Match(JsonElement json);
    }

    private sealed class EqualsFilter(string field, string? expected) : BaseFilter(field)
    {
        public override bool Match(JsonElement root)
        {
            if (!TryGetTopLevelProperty(root, Field, out var val)) return false;
            return val.ValueKind switch
            {
                JsonValueKind.String => string.Equals(val.GetString(), expected, StringComparison.Ordinal),
                JsonValueKind.Number => expected is not null && 
                                        val.TryGetDecimal(out var num) && 
                                        decimal.TryParse(expected, out var exp) && 
                                        num == exp,
                JsonValueKind.True   => string.Equals(expected, "true", StringComparison.OrdinalIgnoreCase),
                JsonValueKind.False  => string.Equals(expected, "false", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }

    private sealed class NotEqualsFilter(string field, string? expected) : BaseFilter(field)
    {
        public override bool Match(JsonElement root)
        {
            if (!TryGetTopLevelProperty(root, Field, out var val))
            {
                return true;
            }
            
            return !(new EqualsFilter(Field, expected).Match(root));
        }
    }

    private sealed class ContainsFilter(string field, string needle) : BaseFilter(field)
    {
        public override bool Match(JsonElement root)
        {
            if (!TryGetTopLevelProperty(root, Field, out var val))
            {
                return false;
            }

            if (val.ValueKind != JsonValueKind.String)
            {
                return false;
            }
            
            string s = val.GetString() ?? string.Empty;
            
            return s.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }

    private sealed class NotContainsFilter(string field, string needle) : BaseFilter(field)
    {
        public override bool Match(JsonElement root) => !new ContainsFilter(Field, needle).Match(root);
    }
}