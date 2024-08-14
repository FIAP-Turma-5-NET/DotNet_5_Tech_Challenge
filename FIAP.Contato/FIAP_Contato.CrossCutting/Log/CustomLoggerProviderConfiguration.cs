using Microsoft.Extensions.Logging;

namespace FIAP_Contato.CrossCutting.Log;

public class CustomLoggerProviderConfiguration
{
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    public int EventId { get; set; } = 0;
}