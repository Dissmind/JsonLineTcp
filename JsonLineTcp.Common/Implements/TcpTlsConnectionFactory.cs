using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using JsonLineTcp.Interefaces;

namespace JsonLineTcp.Implements;

public class TcpTlsConnectionFactory : IConnectionFactory
{
    private readonly string _host;
    private readonly int _port;
    private readonly bool _useTls;
    private readonly bool _validateCertificate;
    
    public TcpTlsConnectionFactory(string host, int port, bool useTls, bool validateCertificate)
    {
        _host = host;
        _port = port;
        _useTls = useTls;
        _validateCertificate = validateCertificate;
    }
    
    
    public async Task<IConnection> ConnectAsync(CancellationToken cancellationToken)
    {
        var client = new TcpClient();

        client.NoDelay = true;

        try
        {
            await client.ConnectAsync(_host, _port, cancellationToken)
                .ConfigureAwait(false);

            Stream stream = client.GetStream();

            if (_useTls)
            {
                stream = await CreateSslStream(stream, cancellationToken);
            }

            return new TcpTlsConnection(client, stream);
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    private async Task<SslStream> CreateSslStream(Stream stream, CancellationToken cancellationToken)
    {
        SslStream sslStream = new SslStream(stream, false, (sender, certificate, chain, sslPolicyErrors) =>
        {
            if (_validateCertificate)
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            }
            
            return true;
        });

        await sslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions()
        {
            TargetHost = _host,
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | 
                                  System.Security.Authentication.SslProtocols.Tls13,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck
        }, cancellationToken)
        .ConfigureAwait(false);
        
        return sslStream;
    }
}