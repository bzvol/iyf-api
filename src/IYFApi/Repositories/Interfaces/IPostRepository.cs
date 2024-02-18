using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IPostRepository
{
    public IEnumerable<PostResponse> GetAllPosts();
    public PostResponse GetPost(ulong id);
    public PostResponse CreatePost(CreatePostRequest value, string userId);
    public PostResponse UpdatePost(ulong id, UpdatePostRequest value, string userId);
    public PostResponse DeletePost(ulong id);
}