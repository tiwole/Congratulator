namespace Congratulator.SharedKernel.Contracts.Options;

public class YandexS3Options
{
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string BucketName { get; set; } = null!;
    public string ServiceUrl { get; set; } = null!;
    public string Region { get; set; } = null!;
}