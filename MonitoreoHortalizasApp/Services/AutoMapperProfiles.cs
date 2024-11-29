using AutoMapper;
using MonitoreoHortalizasApp.entities;

namespace MonitoreoHortalizasApp.Services;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<Sowing, Sowing>();
    }
}