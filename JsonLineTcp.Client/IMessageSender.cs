using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Client.Intrefaces;

public interface IMessageSender
{
    Task<(int valid, int invalid)> SendAllAsync(ILineSource lines, IJsonValidator validator, IConnection connection, CancellationToken cancellationToken);
}