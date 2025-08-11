using System.Net.Sockets;
using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Implements;

public class TcpTlsConnection : IConnection
{
    private readonly TcpClient _client;
    private readonly Stream _stream;
    private static readonly byte NewLine = (byte)'\n'; // 0x0A
    
    
    public TcpTlsConnection(TcpClient client, Stream stream)
    {
        _client = client;
        _stream = stream;
    }
    
    
    public async Task WriteLineAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        await _stream.WriteAsync(data, cancellationToken)
            .ConfigureAwait(false);
        
        await _stream.WriteAsync(new ReadOnlyMemory<byte>(new[] { NewLine }), cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<ReadOnlyMemory<byte>?> ReadLineAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public async Task FlushAsync(CancellationToken cancellationToken)
    {
        await _stream.FlushAsync(cancellationToken)
            .ConfigureAwait(false);
    }
    
    
    public async ValueTask DisposeAsync()
    {
        try
        {
            await _stream.FlushAsync()
                .ConfigureAwait(false);
        }
        catch (Exception)
        {
            // ignored
        }
        
        _stream.Dispose();
        _client.Dispose();
    }
}