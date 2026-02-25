using AutoMapper;
using Congratulator.Infrastructure.Extensions;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Entities;

namespace Congratulator.Infrastructure.AutoMapperProfiles;

public class PersonMappingProfile : Profile
{
    public PersonMappingProfile()
    {
        CreateMap<Person, PersonModel>()
            .ForMember(dest => dest.PhotoPath, opt => opt.MapFrom<PhotoUrlResolver>());
    }
}