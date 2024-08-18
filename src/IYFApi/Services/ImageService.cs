using Amazon.S3;
using Amazon.S3.Model;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Services.Interfaces;

namespace IYFApi.Services;

public class ImageService(IAmazonS3 s3Client) : IImageService
{
    private const string BucketName = "iyfhu";
    private const string FolderName = "content-images";
    private const string Region = "eu-central-1";

    public async Task<ImageUploadResponse> UploadImageAsync(IFormFile file)
    {
        var uid = Guid.NewGuid().ToString();
        var ext = Path.GetExtension(file.FileName);
        var key = $"{FolderName}/{uid}{ext}";

        await using var stream = file.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType,
        };

        await s3Client.PutObjectAsync(putRequest);

        return new ImageUploadResponse
        {
            Url = $"https://{BucketName}.s3.{Region}.amazonaws.com/{key}"
        };
    }
}