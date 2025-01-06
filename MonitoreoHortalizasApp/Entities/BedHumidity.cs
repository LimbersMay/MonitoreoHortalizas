namespace MonitoreoHortalizasApp.entities;

public class BedHumidity
{
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public int Humedad { get; set; }
    
    // Calculates properties
    public string SoloFecha => Fecha.ToString("dd/MM/yyyy");
    
    // Formats the time to show only the hour, minutes and seconds 12 hour format
    public string SoloHora => new DateTime(Hora.Ticks).ToString("hh:mm:ss tt");
}

public class Bed1 : BedHumidity
{
    public string idCama1 { get; set; }
}

public class Bed2 : BedHumidity
{
    public string idCama2 { get; set; }
}

public class Bed3 : BedHumidity
{
    public string idCama3 { get; set; }
}

public class Bed4 : BedHumidity
{
    public string idCama4 { get; set; }
}