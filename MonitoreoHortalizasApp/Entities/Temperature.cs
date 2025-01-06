namespace MonitoreoHortalizasApp.entities;

public class Temperature
{
    public string idTemperatura { get; set; }
    public float Temperatura { get; set; }
    public int Humedad { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    
    // Calculated properties
    public DateOnly SoloFecha => DateOnly.FromDateTime(Fecha);
    public string SoloHora => new DateTime(Hora.Ticks).ToString("hh:mm:ss tt");
}