using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Events.Sensors;

public class GerminationLogUpdatedEvent
{
    public GerminationLog GerminationLog { get; set; }
}