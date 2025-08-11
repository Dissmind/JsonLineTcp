namespace JsonLineTcp.Interefaces;

public interface ILineSource : IAsyncDisposable
{
    IAsyncEnumerable<ReadOnlyMemory<byte>> GetLinesAsync(CancellationToken cancellationToken);
}