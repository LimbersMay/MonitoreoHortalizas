using BlazorBootstrap;
using GestionHortalizasApp.entities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.EnvironmentReadings.Components;

public partial class BarometricPressureTable : ComponentBase, IDisposable
{
    protected List<BarometricPressure> BarometricPressures { get; } = new();
    Grid<BarometricPressure> _grid = default!;
    
    // Services
    [Inject] private IBarometricRepository BarometricRepository { get; set; } = default!;
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        BarometricPressures.AddRange(await BarometricRepository.GetReadings());
        EventAggregator.Subscribe<BarometricPressure>(OnNewBarometricPressureReading);
    }
    
    private async void OnNewBarometricPressureReading(BarometricPressure @event)
    {
        BarometricPressures.Add(@event);
        await _grid.RefreshDataAsync();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        EventAggregator.Unsubscribe<BarometricPressure>(OnNewBarometricPressureReading);
    }
}