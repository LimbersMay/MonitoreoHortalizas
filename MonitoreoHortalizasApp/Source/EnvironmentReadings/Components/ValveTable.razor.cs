using BlazorBootstrap;
using GestionHortalizasApp.entities;
using MonitoreoHortalizasApp.Services;
using Microsoft.AspNetCore.Components;

namespace MonitoreoHortalizasApp.Source.EnvironmentReadings.Components;

public partial class ValveTable : ComponentBase
{
    Grid<Valve> _grid = default!; 
    
    [EditorRequired]
    [Parameter] 
    public List<Valve> Valves { get; set; } = new();
    
    [EditorRequired]
    [Parameter] 
    public string Title { get; set; } = String.Empty;
    
    [Inject]
    public IValveRepository ValveRepository { get; set; } = default!;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    
    public async Task Refresh()
    {
        await _grid.RefreshDataAsync();
    }
}