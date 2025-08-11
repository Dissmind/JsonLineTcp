namespace JsonLineTcp.Client.Common;

public interface IConfigProvider<TConfig>
{
    Task<TConfig> LoadAsync(string path, CancellationToken cancellationToken);
}