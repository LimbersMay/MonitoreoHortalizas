namespace MonitoreoHortalizasApp.entities;

public class Sowing : ICloneable
{
    public string CultivoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string CicloId { get; set; }
    public int Ciclo { get; set; }
    public string NombreCultivo { get; set; } = string.Empty;
    public int Germinacion { get; set; }
    public DateTime FechaSiembra { get; set; } = DateTime.Now;
    public DateTime FechaCosecha { get; set; } = DateTime.Now;
    public string TipoRiego { get; set; } = string.Empty;
    public double Gramaje { get; set; }
    public double AlturaMaxima { get; set; }
    public double AlturaMinima { get; set; }
    public int TemperaturaAmbienteMaxima { get; set; }
    public int TemperaturaAmbienteMinima { get; set; }
    public int HumedadAmbienteMaxima { get; set; }
    public int HumedadAmbienteMinima { get; set; }
    public int HumedadMinimaTierra { get; set; }
    public int HumedadMaximaTierra { get; set; }
    public int PresionBarometricaMaxima { get; set; }
    public int PresionBarometricaMinima { get; set; }
    
    // Calculated fields
    // Format like: January 1, 2021
    public string FechaSiembraFormatted => FechaSiembra.ToString("MMMM dd, yyyy");
    public string FechaCosechaFormatted => FechaCosecha.ToString("MMMM dd, yyyy");
    public int DuracionCiclo => (FechaCosecha - FechaSiembra).Days;
    
    public object Clone()
    {
        return MemberwiseClone();
    }
}