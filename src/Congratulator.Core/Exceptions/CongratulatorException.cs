namespace Congratulator.Core.Exceptions;

public class CongratulatorException(string message) : Exception(message)
{
    public int StatusCode = 500;
}