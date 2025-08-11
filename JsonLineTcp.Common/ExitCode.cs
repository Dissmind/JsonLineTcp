namespace JsonLineTcp.Client;

public enum ExitCode
{
    Success = 0,
    ConfigError = 2,
    SendError = 3,
    Canceled = 130 // SIGINT / Ctrl+C
}