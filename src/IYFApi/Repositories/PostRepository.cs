using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class PostRepository(ApplicationDbContext context) : IPostRepository
{
    public IEnumerable<Post> GetAllPosts()
    {
        return context.Posts;
    }

    public Post GetPost(ulong id)
    {
        return context.Posts.Find(id) ?? throw new KeyNotFoundException(NoPostFoundMessage(id));
    }

    public Post CreatePost(CreatePostRequest value, string userId)
    {
        var post = context.Posts.Add(new Post
        {
            Title = value.Title,
            Content = value.Content,
            CreatedBy = userId
        });
        context.SaveChanges();
        return post.Entity;
    }

    public Post UpdatePost(ulong id, UpdatePostRequest value, string userId)
    {
        var post = context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        post.Title = value.Title;
        post.Content = value.Content;
        post.Status = value.Status;
        post.UpdatedBy = userId;

        var updatedPost = context.Posts.Update(post);
        context.SaveChanges();

        return updatedPost.Entity;
    }

    public Post? DeletePost(ulong id)
    {
        var post = context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        if (post.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft posts.");

        var deletedPost = context.Posts.Remove(post);
        context.SaveChanges();
        return deletedPost.Entity;
    }

    public IEnumerable<Tag> GetTagsForPost(ulong id)
    {
        var post = context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

        return from pt in context.PostsTags
            where pt.PostId == id
            select pt.Tag;
    }

    public IEnumerable<Tag> SetTagsForPost(ulong id, IEnumerable<string> tags)
    {
        var post = context.Posts.Find(id);
        if (post == null) throw new KeyNotFoundException(NoPostFoundMessage(id));

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

        return from pt in context.PostsTags
            where pt.PostId == id
            select pt.Tag;
    }

    public IEnumerable<Post> GetPostsForTag(ulong tagId)
    {
        var tag = context.Tags.Find(tagId);
        if (tag == null) throw new KeyNotFoundException();
        
        return from pt in context.PostsTags
            where pt.TagId == tag.Id
            select pt.Post;
    }

    private static string NoPostFoundMessage(ulong? id) =>
        "The specified post " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}