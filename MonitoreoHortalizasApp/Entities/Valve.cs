namespace GestionHortalizasApp.entities;

public class Valve
{
    public DateTime fechaEncendido { get; set; }
    public DateTime fechaApagado { get; set; }
    public Decimal Volumen { get; set; }
    public string NombreSembrado { get; set; }
    public int CultivoId { get; set; }
    
    // Calculated properties
    public DateOnly SoloFecha => DateOnly.FromDateTime(fechaEncendido);
    public string TiempoEncendido => new DateTime(fechaApagado.Ticks - fechaEncendido.Ticks).ToString("HH:mm:ss");
    public string HoraEncendido => new DateTime(fechaEncendido.Ticks).ToString("hh:mm:ss tt");
    public string HoraApagado => new DateTime(fechaApagado.Ticks).ToString("hh:mm:ss tt");
}