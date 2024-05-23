using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class PostsController(IPostRepository repository) : ControllerBase
{
    [HttpGet]
    [OptionalAdminAuthorizationFilter]
    public async Task<IEnumerable<PostResponse>> GetAllPosts() => (bool)HttpContext.Items["IsAuthorized"]!
        ? await repository.GetAllPostsAuthorized()
        : repository.GetAllPosts();

    [HttpGet("{id}")]
    [OptionalAdminAuthorizationFilter]
    public async Task<PostResponse> GetPost(ulong id) => (bool)HttpContext.Items["IsAuthorized"]!
        ? await repository.GetPostAuthorized(id)
        : repository.GetPost(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest value)
    {
        var post = await repository.CreatePost(value, ((UserRecordFix)HttpContext.Items["User"]!).Uid);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<PostResponse> UpdatePost(ulong id, [FromBody] UpdatePostRequest value) =>
        await repository.UpdatePost(id, value, ((UserRecordFix)HttpContext.Items["User"]!).Uid);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<PostResponse> DeletePost(ulong id) => await repository.DeletePost(id);
}