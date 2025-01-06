using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Events.Sensors;

public class HumidityEvent
{
    public string BedNumber { get; set; }
    public BedHumidity Humidity { get; set; }
}