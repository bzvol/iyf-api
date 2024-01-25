using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _repository;

    public PostsController(IPostRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IEnumerable<Post> GetAllPosts() => _repository.GetAllPosts();

    [HttpGet("{id}")]
    public Post GetPost(ulong id) => _repository.GetPost(id);

    [HttpPost]
    public Post CreatePost([FromBody] CreatePostRequest value)
    {
        var post = _repository.CreatePost(value);
        HttpContext.Response.StatusCode = 201;
        return post;
    }

    [HttpPut("{id}")]
    public Post UpdatePost(ulong id, [FromBody] UpdatePostRequest value) => _repository.UpdatePost(id, value);

    [HttpDelete("{id}")]
    public Post? DeletePost(ulong id) => _repository.DeletePost(id);

    [HttpGet("{id}/tags")]
    public IEnumerable<Tag> GetTagsForPost(ulong id) => _repository.GetTagsForPost(id);

    [HttpPut("{id}/tags")]
    public IEnumerable<Tag> SetTagsForPost(ulong id, [FromBody] IEnumerable<string> tags) =>
        _repository.SetTagsForPost(id, tags);
}