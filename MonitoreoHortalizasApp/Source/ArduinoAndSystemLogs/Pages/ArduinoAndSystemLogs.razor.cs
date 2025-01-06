using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using MonitoreoHortalizasApp.Events.Errors;
using MonitoreoHortalizasApp.Events.Serial;
using MonitoreoHortalizasApp.models;
using MonitoreoHortalizasApp.Services;
using MonitoreoHortalizasApp.services.Events;

namespace MonitoreoHortalizasApp.Source.ArduinoAndSystemLogs.Pages;

public partial class ArduinoAndSystemLogs : ComponentBase, IAsyncDisposable
{
    // Services
    [Inject] public ILogRepository LogRepository { get; set; } = default!;
    [Inject] public IEventAggregator EventAggregator { get; set; } = default!;
    
    private Grid<ArduinoSerialLog> _grid = default!;
    private Grid<string> _errorsGrid = default!;
    
    public List<ArduinoSerialLog> ArduinoSerialLogs { get; set; } = new();
    public List<ArduinoSerialLog> SystemLogs { get; set; } = new();
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        EventAggregator.Subscribe<PrimarySerialDataEvent>(OnArduinoSerialLogReceived);
        EventAggregator.Subscribe<ErrorOccurredEvent>(OnErrorOccurred);
        
        // Load the error logs
        SystemLogs = LogRepository.GetAllLogs().Select(log => new ArduinoSerialLog
        {
            Log = log
        }).ToList();
    }
    
    private async void OnArduinoSerialLogReceived(PrimarySerialDataEvent @event)
    {
        Console.WriteLine("Received log: " + @event.JsonString);
        
        ArduinoSerialLogs.Add(new ArduinoSerialLog
        {
            Log = @event.JsonString
        });

        await _grid.RefreshDataAsync();
    }
    
    private async void OnErrorOccurred(ErrorOccurredEvent e)
    {
        ArduinoSerialLog log = new ArduinoSerialLog
        {
            Log = e.ErrorMessage
        };
        
        SystemLogs.Add(log);
        await _errorsGrid.RefreshDataAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        EventAggregator.Unsubscribe<PrimarySerialDataEvent>(OnArduinoSerialLogReceived);
        EventAggregator.Unsubscribe<ErrorOccurredEvent>(OnErrorOccurred);
        await Task.CompletedTask;
    }
}