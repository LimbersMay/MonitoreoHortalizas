using System.IO;
using System.IO.Ports;
using System.Timers;
using MonitoreoHortalizasApp.Events.Errors;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.Events.Serial;
using MonitoreoHortalizasApp.Models;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.services;

public interface ISerialPortReader
{
    /// <summary>
    /// Get the available serial ports connected to the computer
    /// </summary>
    /// <returns></returns>
    List<string> GetAvailablePorts();

    /// <summary>
    /// Read data from the primary serial port
    /// </summary>
    /// <param name="portName"></param>
    void ReadFromPort(string portName);

    /// <summary>
    /// Read data from the secondary serial port
    /// </summary>
    /// <param name="portName"></param>
    void ReadFromPort2(string portName);
}

public class SerialPortReader : ISerialPortReader
{
    private SerialPort SerialPort { get; set; }
    private SerialPort SerialPort2 { get; set; }

    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger<Runner> _logger;
    private readonly IJsonParser _jsonParser;

    private DateTime _primarySerialPortLastReadTime = DateTime.Now;

    public SerialPortReader(IEventAggregator eventAggregator, ILogger<Runner> logger, IJsonParser jsonParser)
    {
        _eventAggregator = eventAggregator;
        _logger = logger;
        _jsonParser = jsonParser;

        var primarySerialPortTimer = new System.Timers.Timer(120000);

        primarySerialPortTimer.AutoReset = true;
        primarySerialPortTimer.Enabled = true;
    }

    public List<string> GetAvailablePorts()
    {
        return SerialPort.GetPortNames().ToList();
    }

    /// <summary>
    /// Read data from the primary serial port
    /// </summary>
    /// <param name="portName"></param>
    public void ReadFromPort(string portName)
    {
        SerialPort = new SerialPort(portName)
        {
            BaudRate = 9600
        };

        SerialPort.ReadTimeout = 5000;

        // Subscribe to the DataReceived event.
        SerialPort.DataReceived += SerialPortDataReceived;
        SerialPort.ErrorReceived += SerialPortErrorReceived;

        try
        {
            // Now open the port.
            SerialPort.Open();
        }
        catch (UnauthorizedAccessException exception)
        {
            var errorMessage =
                $"No se puede abrir el puerto serie, no disponible u ocupado: {exception}, puerto no disponible o en uso";
            _logger.LogError(errorMessage);
            _eventAggregator.Publish(new ErrorOccurredEvent
            {
                ErrorMessage = errorMessage
            });
        }
        catch (IOException exception)
        {
            var errorMessage =
                $"No se puede abrir el puerto serie, es posible que no esté disponible u ocupado: {portName} {exception}, puerto no disponible o en uso";
            _logger.LogCritical(errorMessage);
            _eventAggregator.Publish(new ErrorOccurredEvent
            {
                ErrorMessage = errorMessage
            });
        }
    }

    /// <summary>
    /// Read data from the secondary serial port
    /// </summary>
    /// <param name="portName"></param>
    public void ReadFromPort2(string portName)
    {
        SerialPort2 = new SerialPort(portName)
        {
            BaudRate = 9600,
        };

        SerialPort2.ReadTimeout = 5000;

        // Subscribe to the DataReceived event.
        SerialPort2.DataReceived += SerialPortDataReceived2;
        SerialPort2.ErrorReceived += SerialPortErrorReceived;

        // Now open the port.
        SerialPort2.Open();
    }

    /// <summary>
    /// Method called when data is received from the primary serial port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var serialPort = (SerialPort)sender;

        var lineRead = String.Empty;

        try
        {
            lineRead = serialPort.ReadLine().Trim();
            _primarySerialPortLastReadTime = DateTime.Now;
            
            _eventAggregator.Publish(new PrimarySerialDataEvent
            {
                JsonString = lineRead
            });
        }
        catch (TimeoutException exception)
        {
            var message = $"Se ha producido un error de tiempo de espera al leer el puerto serie: {exception}";
            _logger.LogError($"TimeoutException: {exception}");

            _eventAggregator.Publish(new ErrorOccurredEvent
            {
                ErrorMessage = message
            });
        }
    }

    /// <summary>
    /// Method called when data is received from the secondary serial port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SerialPortDataReceived2(object sender, SerialDataReceivedEventArgs e)
    {
        var serialPort = (SerialPort)sender;

        var lineRead = String.Empty;

        try
        {
            lineRead = serialPort.ReadLine();
        }
        catch (TimeoutException exception)
        {
            var message = $"Se ha producido un error de tiempo de espera al leer el puerto serie: {exception}";
            _logger.LogError($"TimeoutException: {exception}");

            _eventAggregator.Publish(new ErrorOccurredEvent
            {
                ErrorMessage = message
            });
        }

        // If the line read is not a JSON object, we ignore it
        if (!lineRead.StartsWith("{") || !lineRead.EndsWith("}"))
        {
            return;
        }
        
        _eventAggregator.Publish(new ManualFlowEvent
        {
            JsonString = lineRead
        });
    }

    /// <summary>
    /// Method called when an error occurs in the serial port
    /// An ErrorOccurredEvent is published with the error message
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SerialPortErrorReceived(object sender, SerialErrorReceivedEventArgs e)
    {
        var errorMessage = $"Ocurrió un error en el puerto serie: {e.EventType}: {e}";
        _logger.LogError(errorMessage);
        _eventAggregator.Publish(new ErrorOccurredEvent()
        {
            ErrorMessage = errorMessage
        });
    }

    /// <summary>
    /// Method called every minute to check if the primary serial port is still connected
    /// If no data has been received in the last 2 minutes, the port is closed and reopened
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public void PrimarySerialPortTimerCallback(Object source, ElapsedEventArgs e)
    {
        _eventAggregator.Publish(new PrimarySerialDataEvent()
        {
            JsonString = "Comprobando puerto serie principal"
        });

        // If the last read time is less than 2 minute ago, return
        if (!(_primarySerialPortLastReadTime <= DateTime.Now.AddMinutes(-2)))
        {
            return;
        }
        
        var message = "No se ha recibido información del puerto serie principal en los últimos 2 minutos, intentando reconectar.";

        // Log the message in the logger file and publish an ErrorOccurredEvent
        _logger.LogInformation(message);
        _eventAggregator.Publish(new ErrorOccurredEvent
        {
            ErrorMessage = message
        });

        var portName = SerialPort.PortName;

        /*
         * If the serial port is not open, try to reconnect.
         */
        if (!SerialPort.IsOpen)
        {
            message = $"El puerto serie principal no está abierto, intentando reconectar {SerialPort.PortName}";

            _eventAggregator.Publish(new ErrorOccurredEvent
            {
                ErrorMessage = message
            });

            _logger.LogError(message);

            // Trying to reconnect
            ReadFromPort(portName);
            
            return;
        }

        /*
         * If the serial port is open but no data has been received in the last 2 minutes, we close,
         * dispose and try to reconnect
         */
        SerialPort.Close();
        SerialPort.DiscardInBuffer();
        SerialPort.Dispose();

        // Unsubscribe from the DataReceived event
        SerialPort.DataReceived -= SerialPortDataReceived;
        SerialPort.ErrorReceived -= SerialPortErrorReceived;

        // Reconnect
        ReadFromPort(portName);
    }
}