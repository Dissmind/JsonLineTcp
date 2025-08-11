using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Client.Intrefaces;

public class StreamingMessageSender : IMessageSender
{
    public async Task<(int valid, int invalid)> SendAllAsync(ILineSource lines, IJsonValidator validator, IConnection connection,
        CancellationToken cancellationToken)
    {
        int valid = 0;
        int invalid = 0;

        await foreach (var line in lines.GetLinesAsync(cancellationToken).ConfigureAwait(false))
        {
            if (validator.IsValid(line))
            {
                await connection.WriteLineAsync(line, cancellationToken).ConfigureAwait(false);
                valid++;
            }
            else
            {
                await Console.Error.WriteLineAsync(System.Text.Encoding.UTF8.GetString(line.Span));
                invalid++;
            }
        }
        
        await connection.FlushAsync(cancellationToken).ConfigureAwait(false);
        return (valid, invalid);
    }
}