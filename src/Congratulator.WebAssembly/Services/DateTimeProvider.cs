namespace Congratulator.WebAssembly.Services;

public class DateTimeProvider
{
    public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    public static DateTime UtcNow => DateTime.UtcNow;
}
