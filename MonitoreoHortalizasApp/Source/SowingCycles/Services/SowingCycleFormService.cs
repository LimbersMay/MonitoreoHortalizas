#nullable enable

using MonitoreoHortalizasApp.Entities;

namespace MonitoreoHortalizasApp.Source.SowingCycles.Services;

public class SowingCycleFormService
{
    public SowingCycle? SowingCycle { get; set; }
    
    public void ResetSowingCycle()
    {
        SowingCycle = null;
    }
}