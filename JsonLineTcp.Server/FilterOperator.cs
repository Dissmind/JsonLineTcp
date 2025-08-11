using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace JsonLineTcp.Server;

[JsonConverter(typeof(FileOperatorConverter.FilterOperatorConverter))]
public enum FilterOperator
{
    [EnumMember(Value = "equals")]
    Equals,
    [EnumMember(Value = "not_equals")]
    NotEquals,
    [EnumMember(Value = "contains")]
    Contains,
    [EnumMember(Value = "not_contains")]
    NotContains
}