using GestionHortalizasApp.entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Events.Errors;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.models;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;
using MonitoreoHortalizasApp.Source.EnvironmentReadings.Components;

namespace MonitoreoHortalizasApp.Source.ManualWateringReadings.Pages;

public partial class ManualWateringReadings : ComponentBase, IDisposable
{
    protected List<Valve> Valves { get; } = new();
    [Inject] private IValveRepository ValveRepository { get; set; } = default!;
    [Inject] public IEventAggregator EventAggregator { get; set; }
    [Inject] public ILogger<Runner> Logger { get; set; }
    [Inject] public IJsonParser JsonParser { get; set; }
    
    
    // Reference to the ValveTable component
    public ValveTable ValveTable;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Valves.AddRange(await ValveRepository.GetManualWateringReadings());
        
        EventAggregator.Subscribe<ManualFlowEvent>(OnAddNewReading);
    }
    
    private async void OnAddNewReading(ManualFlowEvent @event)
    {
        var valveReadingResult = JsonParser.TryDeserialize<ValveReading>(@event.JsonString);

        if(!valveReadingResult.IsSuccess)
        {
            Logger.LogError("Error deserializing the Valve object" + valveReadingResult.Error);
            EventAggregator.Publish(new ErrorOccurredEvent { ErrorMessage = "Error deserializing the Valve object" });
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
        
        Valves.Add(waterFlowReading);
        await ValveRepository.AddManualWateringReading(waterFlowReading);
        await ValveTable.Refresh();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        EventAggregator.Unsubscribe<ManualFlowEvent>(OnAddNewReading);
    }
}