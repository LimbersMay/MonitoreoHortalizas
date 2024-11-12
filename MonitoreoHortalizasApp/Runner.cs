using Microsoft.Extensions.Logging;

namespace MonitoreoHortalizasApp;


public class Runner
{
    private readonly ILogger<Runner> _logger;

    public Runner(ILogger<Runner> logger)
    {
        _logger = logger;
    }
}