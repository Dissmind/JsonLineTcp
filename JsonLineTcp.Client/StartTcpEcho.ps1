param(
    [int]$Port = 5555
)

$listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Any, $Port)
$listener.Start()
Write-Host "TCP listening on 0.0.0.0:$Port"

while ($true) {
    $client = $listener.AcceptTcpClient()
    Write-Host "Client connected from $($client.Client.RemoteEndPoint)"
    $stream = $client.GetStream()
    $reader = New-Object System.IO.StreamReader($stream, [System.Text.Encoding]::UTF8, $true, 65536, $false)

    while (($line = $reader.ReadLine()) -ne $null) {
        Write-Host ">> $line"
    }

    $client.Close()
    Write-Host "Client disconnected"
}