using Congratulator.Core.Exceptions;

namespace Congratulator.Infrastructure.Exceptions;

public class InvalidConnectionStringException : CongratulatorException
{
    public InvalidConnectionStringException(string message) : base(message)
    {
        StatusCode = 500;
    }
}