using Congratulator.SharedKernel.Interfaces;

namespace Congratulator.Infrastructure.Extensions;

public class DateTimeProvider : IDateTimeProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public DateTime UtcNow => DateTime.UtcNow;
}
