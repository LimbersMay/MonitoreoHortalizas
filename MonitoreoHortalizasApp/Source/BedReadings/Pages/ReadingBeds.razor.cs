using GestionHortalizasApp.entities;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;
using MonitoreoHortalizasApp.Source.BedReadings.Components;

namespace MonitoreoHortalizasApp.Source.BedReadings.Pages;

public partial class ReadingBeds: ComponentBase, IAsyncDisposable
{
    // Injected dependencies
    [Inject] public IBedRepository BedRepository { get; set; }
    [Inject] public IEventAggregator EventAggregator { get; set; }
    [Inject] public ILogger<Runner> Logger { get; set; }
    [Inject] public IJsonParser JsonParser { get; set; }
    
    // Bed Humidity sensor readings
    public List<BedHumidity> Beds1 { get; } = new();
    public List<BedHumidity> Beds2 { get; } = new();
    public List<BedHumidity> Beds3 { get; } = new();
    public List<BedHumidity> Beds4 { get; } = new();
    
    // References 
    public BedTable BedTable1;
    public BedTable BedTable2;
    public BedTable BedTable3;
    public BedTable BedTable4;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        /*
         * Subscribe to the HumidityEvent to update the table with the new data
         */
        EventAggregator.Subscribe<HumidityEvent>(OnHumidityEventReceived);
        
        /*
         * When the component is initialized, we need to load the data from the database
         * to show it in the table
         */
        var beds1 = await BedRepository.GetBed1HumidityLog();
        var beds2 = await BedRepository.GetBed2HumidityLog();
        var beds3 = await BedRepository.GetBed3HumidityLog();
        var beds4 = await BedRepository.GetBed4HumidityLog();
        
        
        
        foreach (var bed in beds1)
        {
            Beds1.Add(bed);
        }
        
        foreach (var bed in beds2)
        {
            Beds2.Add(bed);
        }
        
        foreach (var bed in beds3)
        {
            Beds3.Add(bed);
        }
        
        foreach (var bed in beds4)
        {
            Beds4.Add(bed);
        }
    }
    
    private async void OnHumidityEventReceived(HumidityEvent @event)
    {
        switch (@event.BedNumber)
        {
            case "1":
                Beds1.Insert(0, @event.Humidity);   
                await BedTable1.Refresh();
                break;
            case "2":
                Beds2.Insert(0, @event.Humidity);
                await BedTable2.Refresh();
                break;
            case "3":
                Beds3.Insert(0, @event.Humidity);
                await BedTable3.Refresh();
                break;
            case "4":
                Beds4.Insert(0, @event.Humidity);
                await BedTable4.Refresh();
                break;
        }
    } 
    
    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<HumidityEvent>(OnHumidityEventReceived);
        await Task.CompletedTask;
    }
}
