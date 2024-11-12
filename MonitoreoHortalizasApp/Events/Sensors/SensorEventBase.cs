namespace MonitoreoHortalizasApp.Events.Sensors;

public class SensorEventBase
{
    public string JsonString { get; set; }
    public DateTime ReceivedAt { get; set; }
    public TimeSpan Time { get; set; }
}