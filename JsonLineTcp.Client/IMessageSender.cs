using JsonLineTcp.Client.Common;

namespace JsonLineTcp.Client.Intrefaces;

public interface IMessageSender
{
    Task<int> SendAllAsync(ILineSource lines, IJsonValidator validator, IConnection connection, CancellationToken cancellationToken);
}