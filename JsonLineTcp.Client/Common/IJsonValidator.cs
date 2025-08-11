namespace JsonLineTcp.Client.Common;

public interface IJsonValidator
{
    bool IsValid(ReadOnlyMemory<byte> json);
}