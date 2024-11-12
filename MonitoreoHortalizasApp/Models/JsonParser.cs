using Newtonsoft.Json;
namespace MonitoreoHortalizasApp.Models;

public interface IJsonParser
{
    Result<T> TryDeserialize<T>(string valueToParse);
}

public class JsonParser: IJsonParser
{
    // Attempts to deserialize the string to the specified generic type T
    public Result<T> TryDeserialize<T>(string valueToParse)
    {
        try
        {
            var value = JsonConvert.DeserializeObject<T>(valueToParse);
            return Result<T>.Success(value);
        }
        catch (JsonReaderException ex)
        {
            var message = $"Error reading the value {valueToParse} | {ex.Message}";
            return HandleError<T>(message);
        }
        catch (JsonSerializationException ex)
        {
            var message = $"Error deserializing the value {valueToParse} | {ex.Message}";
            return HandleError<T>(message);
        }
        catch (Exception ex)
        {
            return HandleError<T>(ex.Message);
        }
    }

    // Handles errors by returning a failed Result<T> with the error message
    private Result<T> HandleError<T>(string message)
    {
        return Result<T>.Failure(message);
    }
}