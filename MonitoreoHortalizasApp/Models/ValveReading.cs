namespace MonitoreoHortalizasApp.models;

public class ValveReading
{
    public long TiempoTranscurridoMilis { set; get; }
    public string NombreSembrado { set; get; }
    public int CultivoId { set; get; }
    public Decimal Volumen;
}