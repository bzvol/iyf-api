using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;
using IYFApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class PostsController(IPostRepository repository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<PostResponse> GetAllPosts() => repository.GetAllPosts();

    [HttpGet("{id}")]
    public PostResponse GetPost(ulong id) => repository.GetPost(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest value)
    {
        var post = repository.CreatePost(value, await AuthService.GetUidFromRequest(Request));
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<PostResponse> UpdatePost(ulong id, [FromBody] UpdatePostRequest value) =>
        repository.UpdatePost(id, value, await AuthService.GetUidFromRequest(Request));

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public PostResponse DeletePost(ulong id) => repository.DeletePost(id);
}