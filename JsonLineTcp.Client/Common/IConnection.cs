namespace JsonLineTcp.Client.Common;

public interface IConnection : IAsyncDisposable
{
    Task WriteLineAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
    Task<ReadOnlyMemory<byte>?> ReadLineAsync(CancellationToken cancellationToken);   
}