using IYFApi.Models;
using IYFApi.Models.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PostsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<Post> Get()
    {
        return _context.Posts;
    }

    [HttpGet("{id}")]
    public Post Get(ulong id)
    {
        return _context.Posts.Find(id) ?? throw new KeyNotFoundException();
    }

    [HttpPost]
    public Post Post([FromBody] Post value)
    {
        var post = _context.Posts.Add(value);
        _context.SaveChanges();
        return post.Entity;
    }

    [HttpDelete("{id}")]
    public Post Delete(ulong id)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException();

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = _context.Posts.Remove(post);
        _context.SaveChanges();
        return deletedPost.Entity;
    }

    [HttpGet("{id}/tags")]
    public IEnumerable<Tag> GetTags(ulong id)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException();

        return from pt in _context.PostsTags
            where pt.PostId == id
            select pt.Tag;
    }

    [HttpPost("{id}/tags")]
    public Post SetTags(ulong id, [FromBody] IEnumerable<string> tags)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException();

        _context.PostsTags.RemoveRange(
            from pt in _context.PostsTags
            where pt.PostId == id
            select pt);

        foreach (var tag in tags)
        {
            var tagEntity = _context.Tags.SingleOrDefault(t => t.Name == tag)
                            ?? _context.Tags.Add(new Tag { Name = tag }).Entity;
            _context.PostsTags.Add(new PostsTag { Post = post, Tag = tagEntity });
        }

        _context.SaveChanges();

        return post;
    }
}