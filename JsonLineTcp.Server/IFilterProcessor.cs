namespace JsonLineTcp.Server;

public interface IFilterProcessor
{
    bool Passes(ReadOnlySpan<byte> utf8);
}