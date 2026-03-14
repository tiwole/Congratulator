using AutoMapper;
using Congratulator.Infrastructure.Extensions;
using Congratulator.SharedKernel.Interfaces;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Entities;

namespace Congratulator.Infrastructure.AutoMapperProfiles;

public class PersonMappingProfile : Profile
{
    public PersonMappingProfile()
    {
        CreateMap<Person, PersonModel>()
            .ForMember(dest => dest.PhotoPath, opt => opt.MapFrom<PhotoUrlResolver>())
            .AfterMap<PersonDateFieldsAction>();
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class PersonDateFieldsAction(IDateTimeProvider dateTimeProvider) : IMappingAction<Person, PersonModel>
{
    public void Process(Person source, PersonModel destination, ResolutionContext context)
    {
        var today = dateTimeProvider.Today;

        var birthdayThisYear = source.BirthDate.AddYears(today.Year - source.BirthDate.Year);
        var nextBirthday = birthdayThisYear < today
            ? birthdayThisYear.AddYears(1)
            : birthdayThisYear;

        destination.Age = today.Year - source.BirthDate.Year -
                          (today.DayOfYear < source.BirthDate.DayOfYear ? 1 : 0);
        destination.NextBirthday = nextBirthday;
        destination.DaysUntilBirthday = nextBirthday.DayNumber - today.DayNumber;
    }
}