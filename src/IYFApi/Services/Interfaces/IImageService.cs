using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Services.Interfaces;

public interface IImageService
{
    public Task<ImageUploadResponse> UploadImageAsync(IFormFile file);
}