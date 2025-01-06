using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Entities;

public class SowingCycle
{
    public string CicloId { get; set; }
    public int Ciclo { get; set; }
    public string Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    
    // Calculated fields
    // Format like: January 1, 2021
    public string FechaInicioFormatted => FechaInicio.ToString("MMMM dd, yyyy");
    public string FechaFinFormatted => FechaFin.ToString("MMMM dd, yyyy");
}