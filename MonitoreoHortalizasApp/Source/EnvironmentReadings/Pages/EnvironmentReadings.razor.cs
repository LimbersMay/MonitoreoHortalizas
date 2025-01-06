using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.entities;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;
using MonitoreoHortalizasApp.Source.EnvironmentReadings.Components;

namespace MonitoreoHortalizasApp.Source.EnvironmentReadings.Pages;

public partial class EnvironmentReadings : ComponentBase, IAsyncDisposable
{
    protected List<Valve> Valves { get; set; } = new();
    
    // Services
    [Inject] private IValveRepository ValveRepository { get; set; } = default!;
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;
    
    // References to child components
    ValveTable ValveTable { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        Valves.AddRange(await ValveRepository.GetReadings());
        
        EventAggregator.Subscribe<Valve>(OnNewValveReading);
        
        /*
         * 
         */
    }
    
    private async void OnNewValveReading(Valve @event)
    {
        Valves.Add(@event);
        await ValveTable.Refresh();
    }
    
    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<Valve>(OnNewValveReading);
        await Task.CompletedTask;
    }
}