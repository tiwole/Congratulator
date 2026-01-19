using AutoMapper;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Entities;

namespace Congratulator.Infrastructure.AutoMapperProfiles;

public class PersonMappingProfile : Profile
{
    public PersonMappingProfile()
    {
        CreateMap<Person, PersonModel>();
    }
}