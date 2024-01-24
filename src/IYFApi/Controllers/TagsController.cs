using IYFApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public TagsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{id}/posts")]
    public IEnumerable<Post> GetPosts(ulong id)
    {
        var tag = _context.Tags.Find(id);
        if (tag == null) throw new KeyNotFoundException();
        
        return from pt in _context.PostsTags
            where pt.TagId == tag.Id
            select pt.Post;
    }
}