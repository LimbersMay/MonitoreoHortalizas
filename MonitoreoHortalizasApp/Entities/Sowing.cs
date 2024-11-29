namespace MonitoreoHortalizasApp.entities;

public class Sowing : ICloneable
{
    public int CultivoId { get; set; }
    public int CicloId { get; set; }
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
    
    public object Clone()
    {
        return MemberwiseClone();
    }
}