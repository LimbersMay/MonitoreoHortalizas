#nullable enable

using System.Diagnostics.CodeAnalysis;
namespace MonitoreoHortalizasApp.Models;

public class Result<T>
{
    // If the result is successful, this will be the value.
    public T? Value { get; private set; }
    public string? Error { get; private set; }

    // This property is used by the compiler to determine if the result is successful or not.
    // If the error is null, the result is successful.
    // If the value is null, the result is a failure.
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error == null;
    
    private Result(T? value, string? error)
    {
        // Ensure that the error and value are not both null.
        if (error != null && value != null)
        {
            throw new InvalidOperationException("Error and Value cannot both be non-null.");
        }

        Value = value;
        Error = error;
    }

    // Factory method for success
    public static Result<T> Success(T value) => new Result<T>(value, null);

    // Factory method for failure
    public static Result<T> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Error message cannot be null or empty", nameof(error));
        }

        return new Result<T>(default, error);
    }
}