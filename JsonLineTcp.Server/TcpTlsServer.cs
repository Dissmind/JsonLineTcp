using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace JsonLineTcp.Server;

public class TcpTlsServer
{
    private readonly IPEndPoint _endPoint;
    private readonly bool _useTls;
    private readonly X509Certificate2? _serverCert;

    public TcpTlsServer(string addr, int port, bool useTls, X509Certificate2? cert)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(addr), port);
        _useTls = useTls;
        _serverCert = cert;
        if (_useTls && _serverCert is null)
        {
            throw new InvalidOperationException("TLS enabled but certificate is null");
        }
    }

    public async Task RunAsync(Func<Stream, Task> handleConnectionAsync, CancellationToken ct)
    {
        var listener = new TcpListener(_endPoint);
        listener.Start();
        Console.WriteLine($"Listening on {_endPoint} (TLS={_useTls})");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(ct)
                    .ConfigureAwait(false);
                
                _ = Task.Run(async () =>
                {
                    using (client)
                    {
                        try
                        {
                            client.NoDelay = true;
                            using var net = client.GetStream();
                            if (_useTls)
                            {
                                using SslStream ssl = new SslStream(net, leaveInnerStreamOpen: false);
                                SslServerAuthenticationOptions opts = new SslServerAuthenticationOptions
                                {
                                    ServerCertificate = _serverCert,
                                    ClientCertificateRequired = false,
                                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
                                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck
                                };
                                await ssl.AuthenticateAsServerAsync(opts, ct)
                                    .ConfigureAwait(false);
                                
                                await handleConnectionAsync(ssl).ConfigureAwait(false);
                            }
                            else
                            {
                                await handleConnectionAsync(net).ConfigureAwait(false);
                            }
                        }
                        catch (OperationCanceledException) { /* ignore */ }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"[conn] error: {ex.Message}");
                        }
                    }
                }, ct);
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    public static X509Certificate2? LoadCertificateFromPem(string? crtPath, string? keyPath)
    {
        if (string.IsNullOrWhiteSpace(crtPath) || string.IsNullOrWhiteSpace(keyPath)) return null;
        var pub = X509Certificate2.CreateFromPemFile(crtPath, keyPath);
        
        return new X509Certificate2(pub.Export(X509ContentType.Pkcs12));
    }
}