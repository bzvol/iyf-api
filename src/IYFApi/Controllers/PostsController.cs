using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class PostsController(IPostRepository repository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<Post> GetAllPosts() => repository.GetAllPosts();

    [HttpGet("{id}")]
    public Post GetPost(ulong id) => repository.GetPost(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IActionResult CreatePost([FromBody] CreatePostRequest value)
    {
        var post = repository.CreatePost(value);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Post UpdatePost(ulong id, [FromBody] UpdatePostRequest value) => repository.UpdatePost(id, value);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Post? DeletePost(ulong id) => repository.DeletePost(id);

    [HttpGet("{id}/tags")]
    public IEnumerable<Tag> GetTagsForPost(ulong id) => repository.GetTagsForPost(id);

    [HttpPut("{id}/tags")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IEnumerable<Tag> SetTagsForPost(ulong id, [FromBody] IEnumerable<string> tags) =>
        repository.SetTagsForPost(id, tags);
}