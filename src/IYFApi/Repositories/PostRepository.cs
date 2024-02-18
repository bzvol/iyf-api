using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
    public IEnumerable<PostResponse> GetAllPosts()
    {
        return context.Posts.Select(ConvertToPostResponse);
    }

    public PostResponse GetPost(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
        return ConvertToPostResponse(post);
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
        
        return ConvertToPostResponse(post.Entity);
    }

    public PostResponse UpdatePost(ulong id, UpdatePostRequest value, string userId)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        post.Title = value.Title;
        post.Content = value.Content;
        post.Status = value.Status;
        post.UpdatedBy = userId;

        var updatedPost = context.Posts.Update(post);
        context.SaveChanges();
        
        SetTagsForPost(updatedPost.Entity.Id, value.Tags);

        return ConvertToPostResponse(updatedPost.Entity);
    }

    public PostResponse DeletePost(ulong id)
    {
        var post = context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = context.Posts.Remove(post);
        context.SaveChanges();
        
        return ConvertToPostResponse(deletedPost.Entity);
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
        Title = post.Title,
        Content = post.Content,
        Tags = from pt in context.PostsTags
            where pt.PostId == post.Id
            select pt.Tag.Name,
        
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