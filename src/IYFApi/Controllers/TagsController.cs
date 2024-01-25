using IYFApi.Models;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IPostRepository _repository;
    
    public TagsController(IPostRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}/posts")]
    public IEnumerable<Post> GetPosts(ulong id) => _repository.GetPostsForTag(id);
}