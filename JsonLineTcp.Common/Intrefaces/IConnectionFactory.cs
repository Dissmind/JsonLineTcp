namespace JsonLineTcp.Interefaces;

public interface IConnectionFactory
{
    Task<IConnection> ConnectAsync(CancellationToken cancellationToken);
}