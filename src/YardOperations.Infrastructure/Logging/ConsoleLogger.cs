using YardOperations.Application.Events;

namespace YardOperations.Infrastructure.Logging;

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
