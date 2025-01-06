using AutoMapper;
using MonitoreoHortalizasApp.entities;
using MonitoreoHortalizasApp.Entities;

namespace MonitoreoHortalizasApp.Services;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<SowingCycle, SowingCycle>();
        CreateMap<Sowing, Sowing>();
        CreateMap<GerminationLog, GerminationLog>();
        CreateMap<SowingLine, SowingLine>();
        
        // BedHumidity
        CreateMap<Bed1, BedHumidity>();
        CreateMap<Bed2, BedHumidity>();
        CreateMap<Bed3, BedHumidity>();
        CreateMap<Bed4, BedHumidity>();
        
        CreateMap<BedHumidity, Bed1>();
        CreateMap<BedHumidity, Bed2>();
        CreateMap<BedHumidity, Bed3>();
        CreateMap<BedHumidity, Bed4>();
    }
}