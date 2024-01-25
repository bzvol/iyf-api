using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;

    public PostRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Post> GetAllPosts()
    {
        return _context.Posts;
    }

    public Post GetPost(ulong id)
    {
        return _context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
    }

    public Post CreatePost(CreatePostRequest value)
    {
        var post = _context.Posts.Add(new Post
        {
            Title = value.Title,
            Body = value.Body,
        });
        _context.SaveChanges();
        return post.Entity;
    }

    public Post UpdatePost(ulong id, UpdatePostRequest value)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        post.Title = value.Title;
        post.Body = value.Body;
        post.Status = value.Status;

        var updatedPost = _context.Posts.Update(post);
        _context.SaveChanges();

        return updatedPost.Entity;
    }

    public Post? DeletePost(ulong id)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = _context.Posts.Remove(post);
        _context.SaveChanges();
        return deletedPost.Entity;
    }

    public IEnumerable<Tag> GetTagsForPost(ulong id)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        return from pt in _context.PostsTags
            where pt.PostId == id
            select pt.Tag;
    }

    public IEnumerable<Tag> SetTagsForPost(ulong id, IEnumerable<string> tags)
    {
        var post = _context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

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

        return from pt in _context.PostsTags
            where pt.PostId == id
            select pt.Tag;
    }

    public IEnumerable<Post> GetPostsForTag(ulong tagId)
    {
        var tag = _context.Tags.Find(tagId);
        if (tag == null) throw new KeyNotFoundException();
        
        return from pt in _context.PostsTags
            where pt.TagId == tag.Id
            select pt.Post;
    }

    private static string NoPostFoundMessage(ulong? id) =>
        "The specified post " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}