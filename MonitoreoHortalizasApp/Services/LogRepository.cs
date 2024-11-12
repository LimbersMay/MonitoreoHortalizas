using System.IO;

namespace MonitoreoHortalizasApp.Services;

public interface ILogRepository
{
    IEnumerable<string> GetAllLogs();
}

public class LogRepository: ILogRepository
{
    private readonly string _logFilePath = "./logs/appLogs.log";

    public IEnumerable<string> GetAllLogs()
    {
        if (File.Exists(_logFilePath))
        {
            return File.ReadAllLines(_logFilePath);
        }
        
        return new List<string>();
    }
}