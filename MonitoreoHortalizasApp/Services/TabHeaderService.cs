namespace MonitoreoHortalizasApp.services;

public class TabHeaderService
{
    public bool IsMainSerialPortSelected { get; set; } = false;
    public bool IsManualSerialPortSelected { get; set; } = false;

    public string MainSerialPortName { get; set; } = string.Empty;
    public string ManualSerialPortName { get; set; } = string.Empty;
}