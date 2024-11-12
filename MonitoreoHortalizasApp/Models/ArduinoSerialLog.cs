namespace MonitoreoHortalizasApp.models;

public class ArduinoSerialLog
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public string Log { get; set; }
}