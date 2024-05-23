using FirebaseAdmin.Auth;
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

    public async Task<IEnumerable<PostAuthorizedResponse>> GetAllPostsAuthorized() => await Task.WhenAll( 
        context.Posts.ToList().Select(ConvertToPostAuthorizedResponse));

    public PostResponse GetPost(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
        if (post.Status != Status.Published) throw new KeyNotFoundException(NoPostFoundMessage(id));
        return ConvertToPostResponse(post);
    }

    public async Task<PostAuthorizedResponse> GetPostAuthorized(ulong id)
    {
        var post = await context.Posts.FindAsync(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
        return await ConvertToPostAuthorizedResponse(post);
    }

    public async Task<PostResponse> CreatePost(CreatePostRequest value, string userId)
    {
        var post = context.Posts.Add(new Post
        {
            Title = value.Title,
            Content = value.Content,
            CreatedBy = userId,
            UpdatedBy = userId
        });
        await context.SaveChangesAsync();

        SetTagsForPost(post.Entity.Id, value.Tags);

        return await ConvertToPostAuthorizedResponse(post.Entity);
    }

    public async Task<PostResponse> UpdatePost(ulong id, UpdatePostRequest value, string userId)
    {
        var post = await context.Posts.FindAsync(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        post.Title = value.Title;
        post.Content = value.Content;
        post.Status = value.Status;
        post.UpdatedBy = userId;
        
        if (post.Status != Status.Published && value.Status == Status.Published)
            post.PublishedAt = DateTime.UtcNow;
        else if (post.Status == Status.Published && value.Status != Status.Published)
            post.PublishedAt = null;

        var updatedPost = context.Posts.Update(post);
        await context.SaveChangesAsync();

        SetTagsForPost(updatedPost.Entity.Id, value.Tags);

        return await ConvertToPostAuthorizedResponse(updatedPost.Entity);
    }

    public async Task<PostResponse> DeletePost(ulong id)
    {
        var post = await context.Posts.FindAsync(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = context.Posts.Remove(post);
        await context.SaveChangesAsync();

        return await ConvertToPostAuthorizedResponse(deletedPost.Entity);
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

    private async Task<PostAuthorizedResponse> ConvertToPostAuthorizedResponse(Post post) => new()
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
            CreatedBy = await FirebaseAuth.DefaultInstance.GetUserAsync(post.CreatedBy),
            UpdatedAt = post.UpdatedAt,
            UpdatedBy = await FirebaseAuth.DefaultInstance.GetUserAsync(post.UpdatedBy),
        }
    };

    private static string NoPostFoundMessage(ulong? id) =>
        "The specified post " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}