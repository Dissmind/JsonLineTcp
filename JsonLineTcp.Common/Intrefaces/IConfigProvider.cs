namespace JsonLineTcp.Interefaces;

public interface IConfigProvider<TConfig>
{
    Task<TConfig> LoadAsync(string path, CancellationToken cancellationToken);
}