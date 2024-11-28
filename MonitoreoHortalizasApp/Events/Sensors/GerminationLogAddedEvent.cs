using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Events.Sensors;

public class GerminationLogAddedEvent
{
    public GerminationLog GerminationLog { get; set; }
}