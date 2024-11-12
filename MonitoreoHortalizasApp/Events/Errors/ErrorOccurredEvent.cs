namespace MonitoreoHortalizasApp.Events.Errors;

/// <summary>
/// The ErrorOccurredEvent event is used to tell the component subscribers that an error occurred.
/// </summary>
public class ErrorOccurredEvent
{
    public string ErrorMessage { get; set; }
}