using System.Text.Json;
using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Implements;


public sealed class JsonValidator : IJsonValidator
{
    public bool IsValid(ReadOnlyMemory<byte> json)
    {
        if (json.IsEmpty)
        {
            return false;
        }

        var opts = new JsonReaderOptions
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Disallow,
            MaxDepth = 10 // TODO: move to config
        };

        try
        {
            var reader = new Utf8JsonReader(json.Span, opts);
            while (reader.Read()) {}
            
            return reader.BytesConsumed == json.Length /*before json data symbols*/ && 
                   reader.TokenType != JsonTokenType.None /*not empty json*/;
        }
        catch
        {
            return false;
        }
    }
}