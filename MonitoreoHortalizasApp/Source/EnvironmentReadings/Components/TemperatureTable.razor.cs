using BlazorBootstrap;
using MonitoreoHortalizasApp.entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.EnvironmentReadings.Components;

public partial class TemperatureTable : ComponentBase, IDisposable
{
    Grid<Temperature> _grid;
    protected List<Temperature> Temperatures { get; } = new();
    [Inject] private ITemperatureRepository TemperatureRepository { get; set; }
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Temperatures.AddRange(await TemperatureRepository.GetReadings());
        
        EventAggregator.Subscribe<Temperature>(OnNewTemperatureReading);
    }
    
    private async void OnNewTemperatureReading(Temperature @event)
    {
        Temperatures.Add(@event);
        await _grid.RefreshDataAsync();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        EventAggregator.Unsubscribe<Temperature>(OnNewTemperatureReading);
    }
}