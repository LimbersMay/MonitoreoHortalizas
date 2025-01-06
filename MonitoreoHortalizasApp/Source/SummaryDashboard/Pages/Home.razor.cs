using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MonitoreoHortalizasApp.Entities;
using MonitoreoHortalizasApp.Events.Errors;
using MonitoreoHortalizasApp.Events.Sensors;
using MonitoreoHortalizasApp.Events.Serial;
using MonitoreoHortalizasApp.models;
using MonitoreoHortalizasApp.Models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.SummaryDashboard.Pages;

public partial class Home: ComponentBase, IAsyncDisposable
{
    // Date range picker
    protected DateTime StartDate { get; set; } = DateTime.Now.Date;
    protected DateTime EndDate { get; set; } = DateTime.Now.Date;
    protected DateTime MinDate { get; set; }
    protected DateTime MaxDate { get; set; } = DateTime.Now.Date;
    
    protected bool DisabledEndDate { get; set; } = true;
    
    // Grid table
    Grid<ArduinoSerialLog> _grid = default!;
    public List<ArduinoSerialLog> ArduinoSerialLogs { get; set; } = new();
    
    // Services
    [Inject] private IBedRepository BedRepository { get; set; } = default!;
    [Inject] private IEventAggregator EventAggregator { get; set; } = default!;
    [Inject] private IJsonParser JsonParser { get; set; } = default!;
    [Inject] private ILogger<Runner> Logger { get; set; } = default!;
    
    // Bed humidities average
    public decimal Bed1AverageHumidity { get; set; }
    public int Bed1HumiditiesLength { get; set; }
    
    public decimal Bed2AverageHumidity { get; set; }
    public int Bed2HumiditiesLength { get; set; }
    
    public decimal Bed3AverageHumidity { get; set; }
    public int Bed3HumiditiesLength { get; set; }
    
    public decimal Bed4AverageHumidity { get; set; }
    public int Bed4HumiditiesLength { get; set; }
    
    // Bed water amount
    public decimal Bed1WaterAmount { get; set; }
    public decimal Bed2WaterAmount { get; set; }
    public decimal Bed3WaterAmount { get; set; }
    public decimal Bed4WaterAmount { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await ReloadDataAsync();
        
        EventAggregator.Subscribe<HumidityEvent>(OnHumidityEventReceived);
        EventAggregator.Subscribe<PrimarySerialDataEvent>(OnArduinoSerialLogReceived);
        
        EventAggregator.Subscribe<ManualWatering>(OnManualWateringReceived);
        EventAggregator.Subscribe<AutomaticWatering>(OnAutomaticWateringReceived);
    }
    
    private async Task ReloadDataAsync()
    {
        // Cargar datos de humedad
        var bed1Humidities = await BedRepository.GetBed1HumiditiesByDates(StartDate, EndDate);
        var bed2Humidities = await BedRepository.GetBed2HumiditiesByDates(StartDate, EndDate);
        var bed3Humidities = await BedRepository.GetBed3HumiditiesByDates(StartDate, EndDate);
        var bed4Humidities = await BedRepository.GetBed4HumiditiesByDates(StartDate, EndDate);

        Bed1AverageHumidity = CalculateAverageHumidity(bed1Humidities);
        Bed1HumiditiesLength = bed1Humidities.Count;

        Bed2AverageHumidity = CalculateAverageHumidity(bed2Humidities);
        Bed2HumiditiesLength = bed2Humidities.Count;

        Bed3AverageHumidity = CalculateAverageHumidity(bed3Humidities);
        Bed3HumiditiesLength = bed3Humidities.Count;

        Bed4AverageHumidity = CalculateAverageHumidity(bed4Humidities);
        Bed4HumiditiesLength = bed4Humidities.Count;

        // Cargar el consumo de agua
        Bed1WaterAmount = await BedRepository.CalculateBed1WaterAmount(StartDate, EndDate);
        Bed2WaterAmount = await BedRepository.CalculateBed2WaterAmount(StartDate, EndDate);
        Bed3WaterAmount = await BedRepository.CalculateBed3WaterAmount(StartDate, EndDate);
        Bed4WaterAmount = await BedRepository.CalculateBed4WaterAmount(StartDate, EndDate);

        await InvokeAsync(StateHasChanged);
    }

    
    private async void StartDateChanged(DateTime? startDate)
    {
        // For the date like: 2024-09-06
        
        if (startDate is null || !startDate.HasValue)
        {
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            DisabledEndDate = true;
            return;
        }

        if (startDate.Value.Date == EndDate.Date)
        {
            DisabledEndDate = true;
        }
        else
        {
            DisabledEndDate = false;
        }

        StartDate = startDate.Value;
        MinDate = startDate.Value;

        await ReloadDataAsync();
    }
    
    private async void EndDateChanged(DateTime? endDate)
    {
        if (endDate is null || !endDate.HasValue)
        {
            EndDate = DateTime.Now.Date;
            return;
        }

        EndDate = endDate.Value;

        await ReloadDataAsync();
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
    
    private decimal CalculateAverageHumidity(List<int> humidities)
    {
        if (humidities.Count == 0) return 0;
        
        var result = Math.Round(humidities.Average(), 2);
        
        return Convert.ToDecimal(result);
    }
    
    private async void OnArduinoSerialLogReceived(PrimarySerialDataEvent @event)
    {
        ArduinoSerialLogs.Add(new ArduinoSerialLog
        {
            Log = @event.JsonString
        });

        await _grid.RefreshDataAsync();
    }
    
    private async void OnManualWateringReceived(ManualWatering @event)
    {
        switch (@event.CultivoId)
        {
            case "739e342b-9d6a-4dc5-a76d-01db4fdf4b15":
                Bed3WaterAmount += @event.Volumen;
                Console.WriteLine("Bed 3 water amount: " + @event.Volumen);
                break;
            case "7b81ada6-80a4-4977-9496-23fa853dbdd3":
                Bed4WaterAmount += @event.Volumen;
                Console.WriteLine("Bed 4 water amount: " + @event.Volumen);
                break;
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnAutomaticWateringReceived(AutomaticWatering @event)
    {
        switch (@event.CultivoId)
        {
            case "7523d7e6-ed0c-46df-837e-8f7afd9c037a":
                Bed1WaterAmount += @event.Volumen;
                Console.WriteLine("Bed 1 water amount: " + @event.Volumen);
                break;
            case "82964903-513d-4c73-891e-f3da0a1cb732":
                Bed2WaterAmount += @event.Volumen;
                Console.WriteLine("Bed 4 water amount: " + @event.Volumen);
                break;
        }
        
        await InvokeAsync(StateHasChanged);
    }
    
    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<HumidityEvent>(OnHumidityEventReceived);
        await Task.CompletedTask;
    }

}
