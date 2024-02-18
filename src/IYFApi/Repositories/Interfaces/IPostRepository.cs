using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IPostRepository
{
    public IEnumerable<Post> GetAllPosts();
    public Post GetPost(ulong id);
    public Post CreatePost(CreatePostRequest value, string userId);
    public Post UpdatePost(ulong id, UpdatePostRequest value, string userId);
    public Post? DeletePost(ulong id);
    public IEnumerable<Tag> GetTagsForPost(ulong id);
    public IEnumerable<Tag> SetTagsForPost(ulong id, IEnumerable<string> tags);
    public IEnumerable<Post> GetPostsForTag(ulong tagId);
}