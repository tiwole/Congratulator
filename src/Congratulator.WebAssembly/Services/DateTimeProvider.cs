namespace Congratulator.WebAssembly.Services;

public class DateTimeProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public DateTime UtcNow => DateTime.UtcNow;
}
