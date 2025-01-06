namespace MonitoreoHortalizasApp.models;

public class ValveReading
{
    public long TiempoTranscurridoMilis { set; get; }
    public string NombreSembrado { set; get; }
    public string CultivoId { set; get; }
    public Decimal Volumen;
}