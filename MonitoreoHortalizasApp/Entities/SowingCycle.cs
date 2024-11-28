using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Entities;

public class SowingCycle
{
    public int CicloId { get; set; }
    public int Ciclo { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}