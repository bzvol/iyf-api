using IYFApi.Models;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class TagsController(IPostRepository repository) : ControllerBase
{
    [HttpGet("{id}/posts")]
    public IEnumerable<Post> GetPosts(ulong id) => repository.GetPostsForTag(id);
}