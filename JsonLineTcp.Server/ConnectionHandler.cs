using System.Text;
using System.Text.Json;

namespace JsonLineTcp.Server;

public class ConnectionHandler
{
    private readonly FilterProcessor _filters;

    public ConnectionHandler(FilterProcessor filters)
    {
        _filters = filters;
    }

    public async Task HandleAsync(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 1 << 4,
            leaveOpen: true);

        while (!ct.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await reader.ReadLineAsync(ct).ConfigureAwait(false);
            }
            catch (IOException)
            {
                break;
            }

            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(line);

            try
            {
                if (_filters.Passes(bytes))
                {
                    using JsonDocument doc = JsonDocument.Parse(bytes);
                    string pretty = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    Console.WriteLine(pretty);
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Bad JSON: {ex.Message}");
                continue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Handle error: {ex.Message}");
                continue;
            }
        }
    }
}