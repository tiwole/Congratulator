using Congratulator.Core.Exceptions;

namespace Congratulator.Infrastructure.Exceptions;

public class NoBucketNameException : CongratulatorException
{
    public NoBucketNameException(string message) : base(message)
    {
        StatusCode = 500;
    }
}