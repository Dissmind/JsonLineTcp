namespace JsonLineTcp.Interefaces;

public interface IJsonValidator
{
    bool IsValid(ReadOnlyMemory<byte> json);
}