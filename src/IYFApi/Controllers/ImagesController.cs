using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class ImagesController(IImageService imageService) : ControllerBase
{
    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<ImageUploadResponse> UploadImage([FromForm] IFormFile file) =>
        await imageService.UploadImageAsync(file);
}