using MonitoreoHortalizasApp.entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Entities;
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
    [Inject] public IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] public ILogger<Runner> Logger { get; set; } = default!;
    [Inject] public IJsonParser JsonParser { get; set; } = default!;
    [Inject] public IGenerateIdService GenerateIdService { get; set; } = default!;
    
    
    // Reference to the ValveTable component
    public ValveTable ValveTable;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Valves.AddRange(await ValveRepository.GetManualWateringReadings());
        
        EventAggregator.Subscribe<ManualWatering>(OnAddNewReading);
    }
    
    private async void OnAddNewReading(ManualWatering @event)
    {
        Valves.Add(@event);
        await ValveTable.Refresh();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        EventAggregator.Unsubscribe<ManualWatering>(OnAddNewReading);
    }
}