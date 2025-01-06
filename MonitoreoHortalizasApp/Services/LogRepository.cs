using System.IO;

namespace MonitoreoHortalizasApp.Services;

public interface ILogRepository
{
    List<string> GetAllLogs();
}

public class LogRepository: ILogRepository
{
    private readonly string _logFilePath = "./logs/appLogs.log";

    public List<string> GetAllLogs()
    {
        if (File.Exists(_logFilePath))
        {
            return File.ReadAllLines(_logFilePath).ToList();
        }
        
        return new List<string>();
    }
}