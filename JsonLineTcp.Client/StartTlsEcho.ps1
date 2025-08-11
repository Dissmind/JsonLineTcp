param(
    [int]$Port = 5555
)

if (-not (Get-Command "openssl" -ErrorAction SilentlyContinue)) {
    Write-Error "OpenSSL notfound"
    exit 1
}

# Если нет сертификата — создаём
if (-not (Test-Path "cert.pem") -or -not (Test-Path "key.pem")) {
    Write-Host "Generating self-signed certificate for CN=localhost..."
    & openssl req -x509 -newkey rsa:2048 -nodes `
        -keyout key.pem -out cert.pem -days 2 -subj "/CN=localhost"
}

Write-Host "Starting TLS echo server on 0.0.0.0:$Port"
& openssl s_server -accept $Port -cert cert.pem -key key.pem -quiet
