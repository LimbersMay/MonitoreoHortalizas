namespace MonitoreoHortalizasApp.Entities;

public class SowingLine
{
    public int NumeroLinea { get; set; }
    public double Gramaje { get; set; }
    public string LineaCultivoId { get; set; }
    public string CultivoId { get; set; }
}