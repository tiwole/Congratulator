using Congratulator.Core.Exceptions;
using Congratulator.Infrastructure.Exceptions;
using Xunit;

namespace Congratulator.XUnitTests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void CongratulatorException_SetsMessageAndDefaultStatusCode()
    {
        var ex = new CongratulatorException("test error");

        Assert.Equal("test error", ex.Message);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void PersonNotFoundException_SetsStatusCode404()
    {
        var ex = new PersonNotFoundException("not found");

        Assert.Equal("not found", ex.Message);
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public void ImageException_SetsStatusCode500()
    {
        var ex = new ImageException("image error");

        Assert.Equal("image error", ex.Message);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void NoBucketNameException_SetsStatusCode500()
    {
        var ex = new NoBucketNameException("no bucket");

        Assert.Equal("no bucket", ex.Message);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void InvalidConnectionStringException_SetsStatusCode500()
    {
        var ex = new InvalidConnectionStringException("bad conn string");

        Assert.Equal("bad conn string", ex.Message);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public void PersonNotFoundException_InheritsFromCongratulatorException()
    {
        var ex = new PersonNotFoundException("test");

        Assert.IsAssignableFrom<CongratulatorException>(ex);
    }

    [Fact]
    public void ImageException_InheritsFromCongratulatorException()
    {
        var ex = new ImageException("test");

        Assert.IsAssignableFrom<CongratulatorException>(ex);
    }
}