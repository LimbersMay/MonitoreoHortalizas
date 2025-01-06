namespace MonitoreoHortalizasApp.entities;

public class BarometricPressure
{ 
    public string idPresionBarometrica { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public Decimal Presion { get; set; }
    public Decimal Temperatura { get; set; }
    public Decimal Altitud { get; set; }
    
    // Calculated properties
    public DateOnly SoloFecha => DateOnly.FromDateTime(Fecha);
    public string SoloHora => new DateTime(Hora.Ticks).ToString("hh:mm:ss tt");
}