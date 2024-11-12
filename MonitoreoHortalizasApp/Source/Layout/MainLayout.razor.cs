using GestionHortalizasApp.entities;
using MonitoreoHortalizasApp.Events.Errors;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.Events.Serial;
using MonitoreoHortalizasApp.models;
using MonitoreoHortalizasApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.Layout;

public partial class MainLayout
{
    // Helper services
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    
    // Repositories
    [Inject] private IBedRepository BedRepository { get; set; } = default!;
    [Inject] private ITemperatureRepository TemperatureRepository { get; set; } = default!;
    [Inject] private IBarometricRepository BarometricRepository { get; set; } = default!;
    [Inject] private IValveRepository ValveRepository { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        // Bed readings
        EventAggregator.Subscribe<PrimarySerialDataEvent>(OnReceivePrimarySerialDataEvent);
        EventAggregator.Subscribe<ManualFlowEvent>(OnReceiveManualFlowEvent);
    }
    
    private async void OnReceivePrimarySerialDataEvent(PrimarySerialDataEvent @event)
    {
        
        var lineRead = @event.JsonString;
        
        // If the line read is not a JSON object, we ignore it
        if (!lineRead.StartsWith("{") || !lineRead.EndsWith("}"))
        {
            return;
        }

        // Try to Deserialize the JSON object to a dictionary 
        var deserializeDataResult = JsonParser.TryDeserialize<Dictionary<string, object>>(lineRead);

        if (!deserializeDataResult.IsSuccess)
        {
            Logger.LogError(deserializeDataResult.Error);
            EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = deserializeDataResult.Error });
            return;
        }

        var deserializeData = deserializeDataResult.Value;

        // Ensure that the key Sensor exists
        if (!deserializeData.ContainsKey("Sensor"))
        {
            var message =
                $"La lectura del sensor: {lineRead} no tiene el atributo de 'Sensor' usado para identificar el tipo de lectura que se recibe (Humedad, temperatura, presion, etc)";

            Logger.LogError(message);
            EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = message });
            return;
        }

        var readingType = deserializeData["Sensor"].ToString();

        // Get the current date and time
        // Format YYYY-MM-DD
        DateTime currentDate = DateTime.Now.Date;
        TimeSpan currentTime = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

        /*
         * Depending on the type of reading, we will parse the JSON object to the corresponding model
         * and save it to the database
         */
        switch (readingType)
        {
            case "temperatura":
                var temperatureReadingResult = JsonParser.TryDeserialize<Temperature>(lineRead);
        
                if (!temperatureReadingResult.IsSuccess)
                {
                    Logger.LogError(temperatureReadingResult.Error);
                    EventAggregator.Publish(new ErrorOccurredEvent {ErrorMessage = temperatureReadingResult.Error});
                    break;
                }
        
                var temperatureReading = temperatureReadingResult.Value;
                
                temperatureReading.Fecha = currentDate;
                temperatureReading.Hora = currentTime;
                
                await TemperatureRepository.AddReading(temperatureReading);
                EventAggregator.Publish(temperatureReading);
                break;

            case "presionBarometrica":

                var barometricPressureReadingResult = JsonParser.TryDeserialize<BarometricPressure>(lineRead);
        
                if (!barometricPressureReadingResult.IsSuccess)
                {
                    Logger.LogError(barometricPressureReadingResult.Error);
                    EventAggregator.Publish(new ErrorOccurredEvent {ErrorMessage = barometricPressureReadingResult.Error});
                    break;
                }
        
                var barometricPressureReading = barometricPressureReadingResult.Value;

                barometricPressureReading.Fecha = currentDate;
                barometricPressureReading.Hora = currentTime;

                await BarometricRepository.AddReading(barometricPressureReading);
                EventAggregator.Publish(barometricPressureReading);
                break;

            case "flujoAgua":

                var valveReadingResult = JsonParser.TryDeserialize<ValveReading>(lineRead);
        
                if (!valveReadingResult.IsSuccess)
                {
                    Logger.LogError(valveReadingResult.Error);
                    EventAggregator.Publish(new ErrorOccurredEvent {ErrorMessage = valveReadingResult.Error});
                    return;
                }
        
                var waterFlowReadingModel = valveReadingResult.Value;

                var waterFlowReading = new Valve()
                {
                    Volumen = waterFlowReadingModel.Volumen,
                    CultivoId = waterFlowReadingModel.CultivoId,
                    NombreSembrado = waterFlowReadingModel.NombreSembrado,
                    fechaEncendido = DateTime.Now.AddMilliseconds(-waterFlowReadingModel.TiempoTranscurridoMilis),
                    fechaApagado = DateTime.Now
                };

                await ValveRepository.AddReading(waterFlowReading);
                EventAggregator.Publish(waterFlowReading);
                break;

            case "humedadCama":

                if (!deserializeData.ContainsKey("NumeroCama"))
                {
                    var errorMessage =
                        $"La lectura del sensor: {lineRead} no tiene el atributo de 'NumeroCama' usado para identificar el número de cama";
                    Logger.LogError(errorMessage);
                    EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = errorMessage });
                    break;
                }
                
                var bedReadingResult = JsonParser.TryDeserialize<BedHumidity>(lineRead);

                if (!bedReadingResult.IsSuccess)
                {
                    Logger.LogError(bedReadingResult.Error);
                    EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = bedReadingResult.Error });
                    return;
                }
        
                var bedHumidity = bedReadingResult.Value;
        
                bedHumidity.Fecha = currentDate;
                bedHumidity.Hora = currentTime;

                switch (deserializeData["NumeroCama"].ToString())
                {
                    case "1":
                        await BedRepository.AddBed1HumidityLog(bedHumidity);
                        EventAggregator.Publish(new HumidityEvent
                        {
                            BedNumber = "1",
                            Humidity = bedHumidity
                        });
                        break;
                    case "2":
                        await BedRepository.AddBed2HumidityLog(bedHumidity);
                        EventAggregator.Publish(new HumidityEvent
                        {
                            BedNumber = "2",
                            Humidity = bedHumidity
                        });
                        break;
                    case "3":
                        await BedRepository.AddBed3HumidityLog(bedHumidity);
                        EventAggregator.Publish(new HumidityEvent
                        {
                            BedNumber = "3",
                            Humidity = bedHumidity
                        });
                        break;
                    case "4":
                        await BedRepository.AddBed4HumidityLog(bedHumidity);
                        EventAggregator.Publish(new HumidityEvent
                        {
                            BedNumber = "4",
                            Humidity = bedHumidity
                        });
                        break;
                }
                break;

            default:
                var message = $"El tipo de lectura: {readingType} con el valor: {lineRead} no es reconocido";
                Logger.LogError(message);
                EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = message });
                break;
        }
        
    }
    
    private async void OnReceiveManualFlowEvent(ManualFlowEvent @event)
    {
        var valveReadingResult = JsonParser.TryDeserialize<ValveReading>(@event.JsonString);
        
        if (!valveReadingResult.IsSuccess)
        {
            Logger.LogError(valveReadingResult.Error);
            EventAggregator.Publish(new ErrorOccurredEvent {ErrorMessage = valveReadingResult.Error});
            return;
        }
        
        var waterFlowReadingModel = valveReadingResult.Value;

        var waterFlowReading = new Valve()
        {
            Volumen = waterFlowReadingModel.Volumen,
            CultivoId = waterFlowReadingModel.CultivoId,
            NombreSembrado = waterFlowReadingModel.NombreSembrado,
            fechaEncendido = DateTime.Now.AddMilliseconds(-waterFlowReadingModel.TiempoTranscurridoMilis),
            fechaApagado = DateTime.Now
        };
        
        await ValveRepository.AddManualWateringReading(waterFlowReading);
    }
}

