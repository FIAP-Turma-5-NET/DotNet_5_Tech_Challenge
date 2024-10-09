using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FIAP_Contato.CrossCutting.Logger;

public class CustomLoggerProvider : ILoggerProvider
{
    private readonly CustomLoggerProviderConfiguration logerConfig;
    private readonly ConcurrentDictionary<string, CustomLogger> loggers = new ConcurrentDictionary<string, CustomLogger>();

    public CustomLoggerProvider(CustomLoggerProviderConfiguration _loggerConfig)
    {
        logerConfig = _loggerConfig;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, name => new CustomLogger(name, logerConfig));
    }

    public void Dispose()
    {
        this.Dispose();
    }
}
