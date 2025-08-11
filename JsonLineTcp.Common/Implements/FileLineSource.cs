using System.Text;
using JsonLineTcp.Interefaces;


namespace JsonLineTcp.Implements;


public class FileLineSource : ILineSource
{
    private readonly StreamReader _reader;
    private readonly FileStream _fileStream;
    
    public FileLineSource(string path)
    {
        _fileStream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read);
        _reader = new StreamReader(
            _fileStream,
            encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            detectEncodingFromByteOrderMarks: true, // delete BOM
            bufferSize: 1 << 4);
    }
    
    public async IAsyncEnumerable<ReadOnlyMemory<byte>> GetLinesAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            string? line = await _reader.ReadLineAsync(cancellationToken)
                .ConfigureAwait(false);

            if (line is null)
            {
                yield break;
            }
            
            byte[] bytes = Encoding.UTF8.GetBytes(line);
            
            yield return bytes;
        }       
    }

    public ValueTask DisposeAsync()
    {
        _fileStream.Dispose();
        _reader.Dispose();

       return ValueTask.CompletedTask;
    }
}