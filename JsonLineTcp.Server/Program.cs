// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using JsonLineTcp.Client;
using JsonLineTcp.Implements;
using JsonLineTcp.Interefaces;
using JsonLineTcp.Server;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: JsonLineTcp.Server <server.config.json>");
    return (int)ExitCode.ConfigError;
}

ServerConfig cfg;
try
{
    await using var fs = File.OpenRead(args[0]);
    cfg = (await JsonSerializer.DeserializeAsync<ServerConfig>(fs, new JsonSerializerOptions
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = false,
        PropertyNameCaseInsensitive = true
    }, cts.Token).ConfigureAwait(false)) ?? throw new InvalidOperationException("Invalid config file");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to read config: {ex.Message}");
    return (int)ExitCode.ConfigError;
}

var cert = cfg.TlsEnabled ? TcpTlsServer.LoadCertificateFromPem(cfg.TlsCrt, cfg.TlsKey) : null;
var server = new TcpTlsServer(cfg.ListenAddr, cfg.ListenPort, cfg.TlsEnabled, cert);

IJsonValidator validator = new JsonValidator();
IFilter[] filters = cfg.Filters.Select(FilterFactory.Create).ToArray();
FilterProcessor processor = new FilterProcessor(validator, filters);
ConnectionHandler handler = new ConnectionHandler(processor);

await server.RunAsync(stream => handler.HandleAsync(stream, cts.Token), cts.Token);

return (int)ExitCode.Success;