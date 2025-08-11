using System.Text.Json;
using JsonLineTcp.Client.Intrefaces;
using JsonLineTcp.Implements;
using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Client;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Need path to config");
            return (int)ExitCode.ConfigError;
        }

        using var cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; 
            cts.Cancel();
        };

        ClientConfig config;

        try
        {
            using var fs = File.OpenRead(args[0]);

            config = (await JsonSerializer.DeserializeAsync<ClientConfig>(fs, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = false,
                PropertyNameCaseInsensitive = true,
            }, cts.Token)) ?? throw new InvalidOperationException("Invalid json config file");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Failed to read config: {e.Message}");
            return (int)ExitCode.ConfigError;
        }

        if (!File.Exists(config.InputPath))
        {
            Console.Error.WriteLine($"Input file not found: {config.InputPath}");
            return (int)ExitCode.ConfigError;
        }

        
        IJsonValidator validator = new JsonValidator();
        await using var lineSource = new FileLineSource(config.InputPath);
        
        IConnectionFactory factory = new TcpTlsConnectionFactory(config.ServerHost, config.ServerPort, 
            config.TlsEnabled, config.TlsValidateCert);

        try
        {
            await using var connection = await factory.ConnectAsync(cts.Token);

            IMessageSender sender = new StreamingMessageSender();
            var (valid, invalid) = await sender.SendAllAsync(lineSource, validator, connection, cts.Token);
            
            Console.Out.WriteLine($"Valid: {valid}, Invalid: {invalid}");
            
            return (int)ExitCode.Success;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Operation canceled");
            return (int)ExitCode.Canceled;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Failed to send data: {e.Message}");
            return (int)ExitCode.SendError;     
        }
    }
}