namespace MonitoreoHortalizasApp.entities;

public class GerminationLog
{
    public int RegistroGerminacionId { get; set; }
    public int CultivoId { get; set; }
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
}