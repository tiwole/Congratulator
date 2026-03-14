namespace Congratulator.Core.Exceptions;

public class ImageException : CongratulatorException
{
    public ImageException(string message) : base(message)
    {
        StatusCode = 500;
    }
}