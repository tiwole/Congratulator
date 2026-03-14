namespace Congratulator.SharedKernel.Interfaces;

public interface IDateTimeProvider
{
    DateOnly Today { get; }
    DateTime UtcNow { get; }
}
