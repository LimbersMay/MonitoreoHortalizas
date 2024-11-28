using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        EventAggregator.Subscribe<ValveReading>(OnReceiveWaterFlowEvent);
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

    private void OnReceiveWaterFlowEvent(ValveReading @event)
    {
        switch (@event.CultivoId)
        {
            case 1:
                Bed1WaterAmount += @event.Volumen;
                break;
            case 2:
                Bed2WaterAmount += @event.Volumen;
                break;
            case 3:
                Bed3WaterAmount += @event.Volumen;
                break;
            case 4:
                Bed4WaterAmount += @event.Volumen;
                break;
        }
    }

    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<HumidityEvent>(OnHumidityEventReceived);
        await Task.CompletedTask;
    }
}
