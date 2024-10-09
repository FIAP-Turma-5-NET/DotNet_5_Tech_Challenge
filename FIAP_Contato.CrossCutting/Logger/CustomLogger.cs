using Microsoft.Extensions.Logging;

namespace FIAP_Contato.CrossCutting.Logger;

public class CustomLogger : ILogger
{
    public static bool LogToFile { get; set; } = true;
    private readonly string _loggerName;
    private readonly CustomLoggerProviderConfiguration _loggerConfig;

    public CustomLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
    {
        _loggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));
        _loggerConfig = loggerConfig ?? throw new ArgumentNullException(nameof(loggerConfig));
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Information;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        var logMessage = $"{logLevel}: {eventId.Id} - {message}";

        if (LogToFile)
        {
            WriteToFile(logMessage);
        }
        if (logLevel == LogLevel.Information)
        {
            Console.WriteLine(logMessage);
        }
    }

    private void WriteToFile(string message)
    {
        string logFilePath = Path.Combine($"{Environment.CurrentDirectory}/Log", $"[FIAPContato-LOG]-{DateTime.Now:yyyy-MM-dd}.txt");

        string directoryPath = Path.GetDirectoryName(logFilePath) ?? throw new InvalidOperationException("Diretório do caminho do arquivo não pode ser nulo.");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (var fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
        using (var streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.WriteLine(message);
        }
    }
}
