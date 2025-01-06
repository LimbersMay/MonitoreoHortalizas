namespace MonitoreoHortalizasApp.Services;

public interface IGenerateIdService
{
    string GenerateId();
}

public class GenerateIdService : IGenerateIdService
{
    public string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }
}