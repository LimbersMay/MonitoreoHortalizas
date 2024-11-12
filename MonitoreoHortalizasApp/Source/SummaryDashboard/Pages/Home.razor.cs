using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.Events.Serial;
using MonitoreoHortalizasApp.models;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.SummaryDashboard.Pages;

public partial class Home: ComponentBase, IAsyncDisposable
{
    Grid<ArduinoSerialLog> _grid = default!;
    public List<ArduinoSerialLog> ArduinoSerialLogs { get; set; } = new();
    
    [Inject] private IBedRepository BedRepository { get; set; } = default!;
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    
    // Bed humidities average
    public double Bed1AverageHumidity { get; set; }
    public int Bed1HumiditiesLength { get; set; }
    
    public double Bed2AverageHumidity { get; set; }
    public int Bed2HumiditiesLength { get; set; }
    
    public double Bed3AverageHumidity { get; set; }
    public int Bed3HumiditiesLength { get; set; }
    
    public double Bed4AverageHumidity { get; set; }
    public int Bed4HumiditiesLength { get; set; }
    
    // Bed water amount
    public double Bed1WaterAmount { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        // Bed humidities
        var bed1Humidities = await BedRepository.GetBed1HumiditiesByDates(DateTime.Now.Date, DateTime.Now.Date);
        var bed2Humidities = await BedRepository.GetBed2HumiditiesByDates(DateTime.Now.Date, DateTime.Now.Date);
        var bed3Humidities = await BedRepository.GetBed3HumiditiesByDates(DateTime.Now.Date, DateTime.Now.Date);
        var bed4Humidities = await BedRepository.GetBed4HumiditiesByDates(DateTime.Now.Date, DateTime.Now.Date);
        
        Bed1AverageHumidity = CalculateAverageHumidity(bed1Humidities);
        Bed1HumiditiesLength = bed1Humidities.Count;
        
        Bed2AverageHumidity = CalculateAverageHumidity(bed2Humidities);
        Bed2HumiditiesLength = bed2Humidities.Count;
        
        Bed3AverageHumidity = CalculateAverageHumidity(bed3Humidities);
        Bed3HumiditiesLength = bed3Humidities.Count;
        
        Bed4AverageHumidity = CalculateAverageHumidity(bed4Humidities);
        Bed4HumiditiesLength = bed4Humidities.Count;
        
        // The amount of water the beds have received
        Bed1WaterAmount = await BedRepository.CalculateBed1WaterAmount();
        
        EventAggregator.Subscribe<HumidityEvent>(OnHumidityEventReceived);
        EventAggregator.Subscribe<PrimarySerialDataEvent>(OnArduinoSerialLogReceived);
    }
    
    private async void OnHumidityEventReceived(HumidityEvent @event)
    { 
        
        switch (@event.BedNumber)
        {
            case "1":
                Bed1AverageHumidity = Math.Round((Bed1AverageHumidity * Bed1HumiditiesLength + @event.Humidity.Humedad) /
                                                 (Bed1HumiditiesLength + 1), 2);
                Bed1HumiditiesLength++;
                break;
            case "2":
                Bed2AverageHumidity = Math.Round((Bed2AverageHumidity * Bed2HumiditiesLength + @event.Humidity.Humedad) /
                                                 (Bed2HumiditiesLength + 1), 2);
                Bed2HumiditiesLength++;
                break;
            case "3":
                Bed3AverageHumidity = Math.Round((Bed3AverageHumidity * Bed3HumiditiesLength + @event.Humidity.Humedad) /
                                                 (Bed3HumiditiesLength + 1), 2);
                Bed3HumiditiesLength++;
                break;
            case "4":
                Bed4AverageHumidity = Math.Round((Bed4AverageHumidity * Bed4HumiditiesLength + @event.Humidity.Humedad) /
                                                 (Bed4HumiditiesLength + 1), 2);
                Bed4HumiditiesLength++;
                break;
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    private double CalculateAverageHumidity(List<int> humidities)
    {
        if (humidities.Count == 0) return 0;
        
        return Math.Round(humidities.Average(), 2);
    }
    
    private async void OnArduinoSerialLogReceived(PrimarySerialDataEvent @event)
    {
        ArduinoSerialLogs.Add(new ArduinoSerialLog
        {
            Log = @event.JsonString
        });

        await _grid.RefreshDataAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<HumidityEvent>(OnHumidityEventReceived);
        await Task.CompletedTask;
    }
}
