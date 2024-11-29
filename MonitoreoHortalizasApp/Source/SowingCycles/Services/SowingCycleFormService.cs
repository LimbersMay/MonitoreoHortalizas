#nullable enable

using MonitoreoHortalizasApp.entities;
using MonitoreoHortalizasApp.Entities;

namespace MonitoreoHortalizasApp.Source.SowingCycles.Services;

public class SowingCycleFormService
{
    public SowingCycle? SowingCycle { get; set; }
    public Sowing? Sowing { get; set; }
    
    public void ResetSowingCycle()
    {
        SowingCycle = null;
    }
    
    public void ResetSowingParameter()
    {
        Sowing = null;
    }
}