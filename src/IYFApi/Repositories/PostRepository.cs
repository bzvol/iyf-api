using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
    public IEnumerable<PostResponse> GetAllPosts() => context.Posts
            .Where(post => post.Status == Status.Published)
            .ToList().Select(ConvertToPostResponse);

    public IEnumerable<PostAuthorizedResponse> GetAllPostsAuthorized() => context.Posts
        .ToList().Select(ConvertToPostAuthorizedResponse);

    public PostResponse GetPost(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
        if (post.Status != Status.Published) throw new KeyNotFoundException(NoPostFoundMessage(id));
        return ConvertToPostResponse(post);
    }

    public PostAuthorizedResponse GetPostAuthorized(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
        return ConvertToPostAuthorizedResponse(post);
    }

    public PostResponse CreatePost(CreatePostRequest value, string userId)
    {
        var post = context.Posts.Add(new Post
        {
            Title = value.Title,
            Content = value.Content,
            CreatedBy = userId
        });
        context.SaveChanges();

        SetTagsForPost(post.Entity.Id, value.Tags);

        return ConvertToPostAuthorizedResponse(post.Entity);
    }

    public PostResponse UpdatePost(ulong id, UpdatePostRequest value, string userId)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        post.Title = value.Title;
        post.Content = value.Content;
        post.Status = value.Status;
        post.UpdatedBy = userId;
        
        if (post.Status != Status.Published && value.Status == Status.Published)
            post.PublishedAt = DateTime.UtcNow;
        else if (post.Status == Status.Published && value.Status != Status.Published)
            post.PublishedAt = null;

        var updatedPost = context.Posts.Update(post);
        context.SaveChanges();

        SetTagsForPost(updatedPost.Entity.Id, value.Tags);

        return ConvertToPostAuthorizedResponse(updatedPost.Entity);
    }

    public PostResponse DeletePost(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = context.Posts.Remove(post);
        context.SaveChanges();

        return ConvertToPostAuthorizedResponse(deletedPost.Entity);
    }

    private void SetTagsForPost(ulong id, IEnumerable<string> tags)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        context.PostsTags.RemoveRange(
            from pt in context.PostsTags
            where pt.PostId == id
            select pt);

        foreach (var tag in tags)
        {
            var tagEntity = context.Tags.SingleOrDefault(t => t.Name == tag)
                            ?? context.Tags.Add(new Tag { Name = tag }).Entity;
            context.PostsTags.Add(new PostsTag { Post = post, Tag = tagEntity });
        }

        context.SaveChanges();
    }

    private PostResponse ConvertToPostResponse(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        Tags = from pt in context.PostsTags
            where pt.PostId == post.Id
            select pt.Tag.Name,
        PublishedAt = post.PublishedAt
    };

    private PostAuthorizedResponse ConvertToPostAuthorizedResponse(Post post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        Tags = from pt in context.PostsTags
            where pt.PostId == post.Id
            select pt.Tag.Name,
        PublishedAt = post.PublishedAt,
        Status = post.Status,
        Metadata = new ObjectMetadata
        {
            CreatedAt = post.CreatedAt,
            CreatedBy = post.CreatedBy,
            UpdatedAt = post.UpdatedAt,
            UpdatedBy = post.UpdatedBy
        }
    };

    private static string NoPostFoundMessage(ulong? id) =>
        "The specified post " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}