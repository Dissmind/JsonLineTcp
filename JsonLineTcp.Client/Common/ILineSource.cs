namespace JsonLineTcp.Client.Common;

public interface ILineSource : IAsyncDisposable
{
    IAsyncEnumerable<ReadOnlyMemory<byte>> GetLinesAsync(CancellationToken cancellationToken);
}