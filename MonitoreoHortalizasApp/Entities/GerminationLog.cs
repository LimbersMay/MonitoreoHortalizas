namespace MonitoreoHortalizasApp.entities;

public class GerminationLog
{
    public string RegistroGerminacionId { get; set; }
    public string CultivoId { get; set; }
    public string NombreCama { get; set; }
    public int Ciclo { get; set; }
    public double TemperaturaAmbiente { get; set; }
    public double HumedadAmbiente { get; set; }
    public int NumeroZurcosGerminados { get; set; }
    public double BroteAlturaMinima { get; set; }
    public double BroteAlturaMaxima { get; set; }
    public double NumeroMortandad { get; set; }
    public double HojasAlturaMaxima { get; set; }
    public double HojasAlturaMinima { get; set; }
    public int Linea { get; set; }
    public string Observaciones { get; set; } = String.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    
    // Calculated properties
    // Format example: 12 Nov 2021 12:00:00 24 hrs
    public string FechaRegistroFormatted => FechaRegistro.ToString("dd MMM yyyy HH:mm:ss");
}