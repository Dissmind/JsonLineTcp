namespace JsonLineTcp.Client.Common;

public interface IConnectionFactory
{
    Task<IConnection> ConnectAsync(string host, int port, CancellationToken cancellationToken);
    Task<IConnection> AcceptAsync(CancellationToken ct);
}