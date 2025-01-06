namespace MonitoreoHortalizasApp.services;

public class TabHeaderService
{
    public bool IsMainSerialPortSelected { get; set; }
    public bool IsManualSerialPortSelected { get; set; }

    public string MainSerialPortName { get; set; } = string.Empty;
    public string ManualSerialPortName { get; set; } = string.Empty;
}