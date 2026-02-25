namespace Congratulator.SharedKernel.Contracts.Options;

public class YandexS3Options
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public string ServiceURL { get; set; }
    public string Region { get; set; }
}