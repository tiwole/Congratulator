using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Congratulator.Infrastructure.Data.ValueConverters;

public class DateOnlyConverter()
    : ValueConverter<DateOnly, DateTime>(x => x.ToDateTime(TimeOnly.MinValue), x => DateOnly.FromDateTime(x))
{
    // DateOnly -> DateTime
    // DateTime -> DateOnly
}