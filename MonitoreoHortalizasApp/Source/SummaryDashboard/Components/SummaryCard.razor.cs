using Microsoft.AspNetCore.Components;

namespace MonitoreoHortalizasApp.Source.SummaryDashboard.Components;

public partial class SummaryCard : ComponentBase
{
    [Parameter] public string Title { get; set; } = default!;
    [Parameter] public decimal Value { get; set; } = default!;
    [Parameter] public DateTime LastRead { get; set; } = default!;
    [Parameter] public string Symbol { get; set; } = default!;
    
    [Parameter] public string Footer { get; set; } = default!;
}